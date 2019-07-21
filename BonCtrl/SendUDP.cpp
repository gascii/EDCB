#include "stdafx.h"
#include "SendUDP.h"
#include "../Common/PathUtil.h"

static const int SNDBUF_SIZE = 3 * 1024 * 1024;

bool CSendUDP::Initialize()
{
	if( m_initialized ){
		return true;
	}
	WSAData wsaData;
	if( WSAStartup(MAKEWORD(2, 2), &wsaData) == 0 ){
		m_initialized = true;
		m_sending = false;
		return true;
	}
	return false;
}

void CSendUDP::UnInitialize()
{
	if( m_initialized ){
		ClearSendAddr();
		WSACleanup();
		m_initialized = false;
	}
}

bool CSendUDP::AddSendAddr(LPCWSTR ip, DWORD dwPort, bool broadcastFlag)
{
	if( m_initialized ){
		SOCKET_DATA Item;
		string ipA;
		WtoUTF8(ip, ipA);
		char szPort[16];
		sprintf_s(szPort, "%d", (WORD)dwPort);
		struct addrinfo hints = {};
		hints.ai_flags = AI_NUMERICHOST;
		hints.ai_socktype = SOCK_DGRAM;
		hints.ai_protocol = IPPROTO_UDP;
		struct addrinfo* result;
		if( getaddrinfo(ipA.c_str(), szPort, &hints, &result) != 0 ){
			return false;
		}
		Item.addrlen = min(result->ai_addrlen, sizeof(Item.addr));
		memcpy(&Item.addr, result->ai_addr, Item.addrlen);
		Item.sock = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
		freeaddrinfo(result);
		if( Item.sock == INVALID_SOCKET ){
			return false;
		}
		//�m���u���b�L���O���[�h��
		ULONG x = 1;
		if( ioctlsocket(Item.sock,FIONBIO, &x) == SOCKET_ERROR ||
		    setsockopt(Item.sock, SOL_SOCKET, SO_SNDBUF, (const char *)&SNDBUF_SIZE, sizeof(SNDBUF_SIZE)) == SOCKET_ERROR ){
			closesocket(Item.sock);
			return false;
		}

		if( broadcastFlag ){
			BOOL b=1;
			setsockopt(Item.sock,SOL_SOCKET, SO_BROADCAST, (char *)&b, sizeof(b));
		}
		SockList.push_back(Item);
		return true;
	}
	return false;
}

void CSendUDP::ClearSendAddr()
{
	for( size_t i=0; i<SockList.size(); i++ ){
		closesocket(SockList[i].sock);
	}
	SockList.clear();
}

bool CSendUDP::StartSend()
{
	if( m_initialized ){
		m_uiSendSize = GetPrivateProfileInt(L"SET", L"UDPPacket", 128, GetModuleIniPath().c_str()) * 188;
		m_sending = true;
		return true;
	}
	return false;
}

bool CSendUDP::AddSendData(BYTE* pbBuff, DWORD dwSize)
{
	if( m_initialized == false || m_sending == false ){
		return false;
	}
	for( DWORD dwRead=0; dwRead<dwSize; ){
		//�y�C���[�h�����BBonDriver_UDP�ɑ���ꍇ�͎�M�T�C�Y48128�ȉ��łȂ���΂Ȃ�Ȃ�
		int iSendSize = min(max((int)m_uiSendSize, 188), (int)(dwSize - dwRead));
		for( size_t i=0; i<SockList.size(); i++ ){
			int iRet = sendto(SockList[i].sock, (char*)(pbBuff + dwRead), iSendSize, 0, (struct sockaddr *)&SockList[i].addr, (int)SockList[i].addrlen);
			if( iRet == SOCKET_ERROR ){
				if( WSAGetLastError() == WSAEWOULDBLOCK ){
					//���M�������ǂ�����SNDBUF_SIZE�Ŏw�肵���o�b�t�@���s���Ă��܂���
					//�ш悪����Ȃ��Ƃ��͂ǂ����~���Ă��h���b�v���邵���Ȃ��̂ŁASleep()�ɂ��t���[����͂��Ȃ�
					OutputDebugString(L"Dropped\r\n");
				}
			}
		}
		dwRead += iSendSize;
	}
	return true;
}
