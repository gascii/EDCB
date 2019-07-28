#include "stdafx.h"
#include "BonCtrl.h"

#include "../Common/CommonDef.h"
#include "../Common/TimeUtil.h"
#include "../Common/SendCtrlCmd.h"

CBonCtrl::CBonCtrl(void)
{
	this->statusSignalLv = 0.0f;
	this->viewSpace = -1;
	this->viewCh = -1;
	this->chScanIndexOrStatus = ST_STOP;
	this->epgCapIndexOrStatus = ST_STOP;
	this->enableLiveEpgCap = FALSE;
	this->enableRecEpgCap = FALSE;

	this->epgCapBackBSBasic = TRUE;
	this->epgCapBackCS1Basic = TRUE;
	this->epgCapBackCS2Basic = TRUE;
	this->epgCapBackCS3Basic = FALSE;
	this->epgCapBackStartWaitSec = 30;
}


CBonCtrl::~CBonCtrl(void)
{
	CloseBonDriver();

	StopChScan();
}

//BonDriver�t�H���_���w��
//�����F
// bonDriverFolderPath		[IN]BonDriver�t�H���_�p�X
void CBonCtrl::SetBonDriverFolder(
	LPCWSTR bonDriverFolderPath
)
{
	this->bonUtil.SetBonDriverFolder(bonDriverFolderPath);
}

//BonDriver�t�H���_��BonDriver_*.dll���
//�߂�l�F
// �����ł���BonDriver�ꗗ
vector<wstring> CBonCtrl::EnumBonDriver()
{
	return this->bonUtil.EnumBonDriver();
}

//BonDriver�����[�h���ă`�����l�����Ȃǂ��擾�i�t�@�C�����Ŏw��j
//�߂�l�F
// �G���[�R�[�h
//�����F
// bonDriverFile	[IN]EnumBonDriver�Ŏ擾���ꂽBonDriver�̃t�@�C����
DWORD CBonCtrl::OpenBonDriver(
	LPCWSTR bonDriverFile,
	int openWait,
	DWORD tsBuffMaxCount
)
{
	CloseBonDriver();
	DWORD ret = ERR_FALSE;
	if( this->bonUtil.OpenBonDriver(bonDriverFile, [=](BYTE* data, DWORD size, DWORD remain) { RecvCallback(data, size, remain, tsBuffMaxCount); },
	                                [=](float signalLv, int space, int ch) { StatusCallback(signalLv, space, ch); }, openWait) ){
		wstring bonFile = this->bonUtil.GetOpenBonDriverFileName();
		ret = NO_ERR;
		//��̓X���b�h�N��
		this->analyzeStopFlag = false;
		this->analyzeThread = thread_(AnalyzeThread, this);

		this->tsOut.SetBonDriver(bonFile);
		fs_path settingPath = GetSettingPath();
		wstring tunerName = this->bonUtil.GetTunerName();
		CheckFileName(tunerName);
		this->chUtil.LoadChSet(fs_path(settingPath).append(fs_path(bonFile).stem().concat(L"(" + tunerName + L").ChSet4.txt").native()).native(),
		                       fs_path(settingPath).append(L"ChSet5.txt").native());
	}

	return ret;
}

//���[�h����BonDriver�̃t�@�C�������擾����i���[�h�������Ă��邩�̔���j
//���X���b�h�Z�[�t
//�߂�l�F
// TRUE�i�����j�FFALSE�iOpen�Ɏ��s���Ă���j
//�����F
// bonDriverFile		[OUT]BonDriver�̃t�@�C����(NULL��)
BOOL CBonCtrl::GetOpenBonDriver(
	wstring* bonDriverFile
	)
{
	BOOL ret = FALSE;

	wstring strBonDriverFile = this->bonUtil.GetOpenBonDriverFileName();
	if( strBonDriverFile.empty() == false ){
		ret = TRUE;
		if( bonDriverFile != NULL ){
			*bonDriverFile = strBonDriverFile;
		}
	}

	return ret;
}

//�`�����l���ύX
//�߂�l�F
// �G���[�R�[�h
//�����F
// space			[IN]�ύX�`�����l����Space
// ch				[IN]�ύX�`�����l���̕���Ch
DWORD CBonCtrl::SetCh(
	DWORD space,
	DWORD ch
)
{
	if( this->tsOut.IsRec() == TRUE ){
		return ERR_FALSE;
	}

	return ProcessSetCh(space, ch, FALSE, TRUE);
}

//�`�����l���ύX
//�߂�l�F
// �G���[�R�[�h
//�����F
// ONID			[IN]�ύX�`�����l����orignal_network_id
// TSID			[IN]�ύX�`�����l����transport_stream_id
// SID			[IN]�ύX�`�����l����service_id
DWORD CBonCtrl::SetCh(
	WORD ONID,
	WORD TSID,
	WORD SID
)
{
	if( this->tsOut.IsRec() == TRUE ){
		return ERR_FALSE;
	}

	DWORD space=0;
	DWORD ch=0;

	DWORD ret = ERR_FALSE;
	if( this->chUtil.GetCh( ONID, TSID, SID, space, ch ) == TRUE ){
		ret = ProcessSetCh(space, ch, FALSE, TRUE);
	}

	return ret;
}

DWORD CBonCtrl::ProcessSetCh(
	DWORD space,
	DWORD ch,
	BOOL chScan,
	BOOL restartEpgCapBack
	)
{
	DWORD spaceNow=0;
	DWORD chNow=0;

	DWORD ret = ERR_FALSE;
	if( this->bonUtil.GetOpenBonDriverFileName().empty() == false ){
		ret = NO_ERR;
		DWORD elapsed;
		if( this->bonUtil.GetNowCh(&spaceNow, &chNow) == false || space != spaceNow || ch != chNow || this->tsOut.IsChUnknown(&elapsed) && elapsed > 15000 ){
			if( restartEpgCapBack ){
				StopBackgroundEpgCap();
			}
			this->tsOut.SetChChangeEvent(chScan);
			_OutputDebugString(L"SetCh space %d, ch %d", space, ch);
			ret = this->bonUtil.SetCh(space, ch) ? NO_ERR : ERR_FALSE;

			if( restartEpgCapBack ){
				StartBackgroundEpgCap();
			}
		}
	}else{
		OutputDebugString(L"Err GetNowCh");
	}
	return ret;
}

//�`�����l���ύX�����ǂ���
//���X���b�h�Z�[�t
//�߂�l�F
// TRUE�i�ύX���j�AFALSE�i�����j
BOOL CBonCtrl::IsChChanging(BOOL* chChgErr)
{
	if( chChgErr != NULL ){
		*chChgErr = FALSE;
	}
	DWORD elapsed;
	if( this->tsOut.IsChUnknown(&elapsed) && elapsed != MAXDWORD ){
		if( elapsed > 15000 ){
			//�^�C���A�E�g����
			if( chChgErr != NULL ){
				*chChgErr = TRUE;
			}
			return FALSE;
		}
		return TRUE;
	}
	return FALSE;
}

//���݂̃X�g���[����ID���擾����
//�߂�l�F
// TRUE�i�����j�AFALSE�i���s�j
//�����F
// ONID		[OUT]originalNetworkID
// TSID		[OUT]transportStreamID
BOOL CBonCtrl::GetStreamID(
	WORD* ONID,
	WORD* TSID
	)
{
	return this->tsOut.GetStreamID(ONID, TSID);
}

//���[�h���Ă���BonDriver�̊J��
void CBonCtrl::CloseBonDriver()
{
	StopBackgroundEpgCap();

	StopEpgCap();

	if( this->analyzeThread.joinable() ){
		this->analyzeStopFlag = true;
		this->analyzeEvent.Set();
		this->analyzeThread.join();
	}

	this->bonUtil.CloseBonDriver();
	this->packetInit.ClearBuff();
	this->tsBuffList.clear();
	this->tsFreeList.clear();
}

void CBonCtrl::RecvCallback(BYTE* data, DWORD size, DWORD remain, DWORD tsBuffMaxCount)
{
	BYTE* outData;
	DWORD outSize;
	if( data != NULL && size != 0 && this->packetInit.GetTSData(data, size, &outData, &outSize) ){
		CBlockLock lock(&this->buffLock);
		while( outSize != 0 ){
			if( this->tsFreeList.empty() ){
				//�o�b�t�@�𑝂₷
				if( this->tsBuffList.size() > tsBuffMaxCount ){
					for( auto itr = this->tsBuffList.begin(); itr != this->tsBuffList.end(); (itr++)->clear() );
					this->tsFreeList.splice(this->tsFreeList.end(), this->tsBuffList);
				}else{
					this->tsFreeList.push_back(vector<BYTE>());
					this->tsFreeList.back().reserve(48128);
				}
			}
			DWORD insertSize = min(48128 - (DWORD)this->tsFreeList.front().size(), outSize);
			this->tsFreeList.front().insert(this->tsFreeList.front().end(), outData, outData + insertSize);
			if( this->tsFreeList.front().size() == 48128 ){
				this->tsBuffList.splice(this->tsBuffList.end(), this->tsFreeList, this->tsFreeList.begin());
			}
			outData += insertSize;
			outSize -= insertSize;
		}
	}
	if( remain == 0 ){
		this->analyzeEvent.Set();
	}
}

void CBonCtrl::StatusCallback(float signalLv, int space, int ch)
{
	CBlockLock lock(&this->buffLock);
	this->statusSignalLv = signalLv;
	this->viewSpace = space;
	this->viewCh = ch;
}

void CBonCtrl::AnalyzeThread(CBonCtrl* sys)
{
	std::list<vector<BYTE>> data;

	while( sys->analyzeStopFlag == false ){
		//�o�b�t�@����f�[�^���o��
		float signalLv;
		{
			CBlockLock lock(&sys->buffLock);
			if( data.empty() == false ){
				//�ԋp
				data.front().clear();
				sys->tsFreeList.splice(sys->tsFreeList.end(), data);
			}
			if( sys->tsBuffList.empty() == false ){
				data.splice(data.end(), sys->tsBuffList, sys->tsBuffList.begin());
			}
			signalLv = sys->statusSignalLv;
		}
		sys->tsOut.SetSignalLevel(signalLv);
		if( data.empty() == false ){
			sys->tsOut.AddTSBuff(&data.front().front(), (DWORD)data.front().size());
		}else{
			sys->analyzeEvent.WaitOne(1000);
		}
	}
}

//�T�[�r�X�ꗗ���擾����
//�߂�l�F
// �G���[�R�[�h
//�����F
// serviceList				[OUT]�T�[�r�X���̃��X�g
DWORD CBonCtrl::GetServiceList(
	vector<CH_DATA4>* serviceList
	)
{
	return this->chUtil.GetEnumService(serviceList);
}

//TS�X�g���[������p�R���g���[�����쐬����
//�߂�l�F
// ���䎯��ID
//�����F
// sendUdpTcp	[IN]UDP/TCP���M�p�ɂ���
DWORD CBonCtrl::CreateServiceCtrl(
	BOOL sendUdpTcp
	)
{
	return this->tsOut.CreateServiceCtrl(sendUdpTcp);
}

//TS�X�g���[������p�R���g���[�����쐬����
//�߂�l�F
// �G���[�R�[�h
//�����F
// id			[IN]���䎯��ID
BOOL CBonCtrl::DeleteServiceCtrl(
	DWORD id
	)
{
	return this->tsOut.DeleteServiceCtrl(id);
}

//����Ώۂ̃T�[�r�X��ݒ肷��
BOOL CBonCtrl::SetServiceID(
	DWORD id,
	WORD serviceID
	)
{
	return this->tsOut.SetServiceID(id,serviceID);
}

BOOL CBonCtrl::GetServiceID(
	DWORD id,
	WORD* serviceID
	)
{
	return this->tsOut.GetServiceID(id,serviceID);
}

//UDP�ő��M���s��
//�߂�l�F
// TRUE�i�����j�AFALSE�i���s�j
//�����F
// id			[IN]���䎯��ID
// sendList		[IN/OUT]���M�惊�X�g�BNULL�Œ�~�BPort�͎��ۂɑ��M�Ɏg�p����Port���Ԃ�B�B
BOOL CBonCtrl::SendUdp(
	DWORD id,
	vector<NW_SEND_INFO>* sendList
	)
{
	return this->tsOut.SendUdp(id,sendList);
}

//TCP�ő��M���s��
//�߂�l�F
// TRUE�i�����j�AFALSE�i���s�j
//�����F
// id			[IN]���䎯��ID
// sendList		[IN/OUT]���M�惊�X�g�BNULL�Œ�~�BPort�͎��ۂɑ��M�Ɏg�p����Port���Ԃ�B
BOOL CBonCtrl::SendTcp(
	DWORD id,
	vector<NW_SEND_INFO>* sendList
	)
{
	return this->tsOut.SendTcp(id,sendList);
}

BOOL CBonCtrl::StartSave(
	const SET_CTRL_REC_PARAM& recParam,
	const vector<wstring>& saveFolderSub,
	int writeBuffMaxCount
)
{
	if( this->tsOut.StartSave(recParam, saveFolderSub, writeBuffMaxCount) ){
		StartBackgroundEpgCap();
		return TRUE;
	}
	return FALSE;
}

BOOL CBonCtrl::EndSave(
	DWORD id,
	BOOL* subRecFlag
	)
{
	return this->tsOut.EndSave(id, subRecFlag);
}

//�X�N�����u�����������̓���ݒ�
//�߂�l�F
// TRUE�i�����j�AFALSE�i���s�j
//�����F
// enable		[IN] TRUE�i��������j�AFALSE�i�������Ȃ��j
BOOL CBonCtrl::SetScramble(
	DWORD id,
	BOOL enable
	)
{
	return this->tsOut.SetScramble(id, enable);
}

//�����ƃf�[�^�����܂߂邩�ǂ���
//�����F
// id					[IN]���䎯��ID
// enableCaption		[IN]������ TRUE�i�܂߂�j�AFALSE�i�܂߂Ȃ��j
// enableData			[IN]�f�[�^������ TRUE�i�܂߂�j�AFALSE�i�܂߂Ȃ��j
void CBonCtrl::SetServiceMode(
	DWORD id,
	BOOL enableCaption,
	BOOL enableData
	)
{
	this->tsOut.SetServiceMode(id, enableCaption, enableData);
}

//�G���[�J�E���g���N���A����
void CBonCtrl::ClearErrCount(
	DWORD id
	)
{
	this->tsOut.ClearErrCount(id);
}

//�h���b�v�ƃX�N�����u���̃J�E���g���擾����
//�����F
// drop				[OUT]�h���b�v��
// scramble			[OUT]�X�N�����u����
void CBonCtrl::GetErrCount(
	DWORD id,
	ULONGLONG* drop,
	ULONGLONG* scramble
	)
{
	this->tsOut.GetErrCount(id, drop, scramble);
}

//�`�����l���X�L�������J�n����
//�߂�l�F
// TRUE�i�����j�AFALSE�i���s�j
BOOL CBonCtrl::StartChScan()
{
	if( this->tsOut.IsRec() == TRUE ){
		return FALSE;
	}
	if( this->chScanThread.joinable() && this->chScanIndexOrStatus >= 0 ){
		return FALSE;
	}

	StopBackgroundEpgCap();
	StopChScan();

	if( this->bonUtil.GetOpenBonDriverFileName().empty() == false ){
		this->chScanIndexOrStatus = ST_COMPLETE;
		this->chScanChkList.clear();
		vector<pair<wstring, vector<wstring>>> spaceList = this->bonUtil.GetOriginalChList();
		for( size_t i = 0; i < spaceList.size(); i++ ){
			for( size_t j = 0; j < spaceList[i].second.size(); j++ ){
				if( spaceList[i].second[j].empty() == false ){
					CHK_CH_INFO item;
					item.space = (DWORD)i;
					item.spaceName = spaceList[i].first;
					item.ch = (DWORD)j;
					item.chName = spaceList[i].second[j];
					this->chScanChkList.push_back(item);
					this->chScanIndexOrStatus = 0;
				}
			}
		}
		//��M�X���b�h�N��
		this->chScanStopEvent.Reset();
		this->chScanThread = thread_(ChScanThread, this);
		return TRUE;
	}

	return FALSE;
}

//�`�����l���X�L�������L�����Z������
void CBonCtrl::StopChScan()
{
	if( this->chScanThread.joinable() ){
		this->chScanStopEvent.Set();
		this->chScanThread.join();
	}
}

//�`�����l���X�L�����̏�Ԃ��擾����
//�߂�l�F
// �X�e�[�^�X
//�����F
// space		[OUT]�X�L�������̕���CH��space
// ch			[OUT]�X�L�������̕���CH��ch
// chName		[OUT]�X�L�������̕���CH�̖��O
// chkNum		[OUT]�`�F�b�N�ς݂̐�
// totalNum		[OUT]�`�F�b�N�Ώۂ̑���
CBonCtrl::JOB_STATUS CBonCtrl::GetChScanStatus(
	DWORD* space,
	DWORD* ch,
	wstring* chName,
	DWORD* chkNum,
	DWORD* totalNum
	)
{
	int indexOrStatus = this->chScanIndexOrStatus;
	if( indexOrStatus < 0 ){
		return (JOB_STATUS)indexOrStatus;
	}
	if( space != NULL ){
		*space = this->chScanChkList[indexOrStatus].space;
	}
	if( ch != NULL ){
		*ch = this->chScanChkList[indexOrStatus].ch;
	}
	if( chName != NULL ){
		*chName = this->chScanChkList[indexOrStatus].chName;
	}
	if( chkNum != NULL ){
		*chkNum = indexOrStatus;
	}
	if( totalNum != NULL ){
		*totalNum = (DWORD)this->chScanChkList.size();
	}
	return ST_WORKING;
}

void CBonCtrl::ChScanThread(CBonCtrl* sys)
{
	//TODO: chUtil��const�ɕۂ��Ă��Ȃ��̂ŃX���b�h���S���͔j�]���Ă���B�X�L�����������̖��Ȃ̂ŏC���͂��Ȃ����v����
	sys->chUtil.Clear();

	fs_path settingPath = GetSettingPath();
	fs_path bonFile = sys->bonUtil.GetOpenBonDriverFileName();
	wstring tunerName = sys->bonUtil.GetTunerName();
	CheckFileName(tunerName);

	wstring chSet4 = fs_path(settingPath).append(bonFile.stem().concat(L"(" + tunerName + L").ChSet4.txt").native()).native();
	wstring chSet5 = fs_path(settingPath).append(L"ChSet5.txt").native();

	if( sys->chScanIndexOrStatus < 0 ){
		sys->chUtil.SaveChSet(chSet4, chSet5);
		return;
	}

	fs_path iniPath = GetCommonIniPath().replace_filename(L"BonCtrl.ini");

	DWORD chChgTimeOut = GetPrivateProfileInt(L"CHSCAN", L"ChChgTimeOut", 9, iniPath.c_str());
	DWORD serviceChkTimeOut = GetPrivateProfileInt(L"CHSCAN", L"ServiceChkTimeOut", 8, iniPath.c_str());


	BOOL chkNext = TRUE;
	DWORD startTime = 0;

	for(;;){
		if( sys->chScanStopEvent.WaitOne(chkNext ? 0 : 1000) ){
			//�L�����Z�����ꂽ
			sys->chScanIndexOrStatus = ST_CANCEL;
			break;
		}
		if( chkNext == TRUE ){
			sys->ProcessSetCh(sys->chScanChkList[sys->chScanIndexOrStatus].space,
			                  sys->chScanChkList[sys->chScanIndexOrStatus].ch, TRUE, FALSE);
			startTime = GetTickCount();
			chkNext = FALSE;
		}else{
			DWORD elapsed;
			if( sys->tsOut.IsChUnknown(&elapsed) ){
				if( elapsed > chChgTimeOut * 1000 ){
					//�`�����l���؂�ւ���chChgTimeOut�b�ȏォ�����Ă�̂Ŗ��M���Ɣ��f
					OutputDebugString(L"��AutoScan Ch Change timeout\r\n");
					chkNext = TRUE;
				}
			}else{
				if( GetTickCount() - startTime > (chChgTimeOut + serviceChkTimeOut) * 1000 ){
					//�`�����l���؂�ւ������������ǃT�[�r�X�ꗗ�Ƃ�Ȃ��̂Ŗ��M���Ɣ��f
					OutputDebugString(L"��AutoScan GetService timeout\r\n");
					chkNext = TRUE;
				}else{
					//�T�[�r�X�ꗗ�̎擾���s��
					sys->tsOut.GetServiceListActual([sys, &chkNext](DWORD serviceListSize, SERVICE_INFO* serviceList) {
						if( serviceListSize > 0 ){
							//�ꗗ�̎擾���ł���
							for( int currSID = 0; currSID < 0x10000; ){
								//ServiceID���ɒǉ�
								int nextSID = 0x10000;
								for( DWORD i = 0; i < serviceListSize; i++ ){
									WORD serviceID = serviceList[i].service_id;
									if( serviceID == currSID && serviceList[i].extInfo && serviceList[i].extInfo->service_name ){
										if( wcslen(serviceList[i].extInfo->service_name) > 0 ){
											sys->chUtil.AddServiceInfo(sys->chScanChkList[sys->chScanIndexOrStatus].space,
											                           sys->chScanChkList[sys->chScanIndexOrStatus].ch,
											                           sys->chScanChkList[sys->chScanIndexOrStatus].chName, &(serviceList[i]));
										}
									}
									if( serviceID > currSID && serviceID < nextSID ){
										nextSID = serviceID;
									}
								}
								currSID = nextSID;
							}
							chkNext = TRUE;
						}
					});
				}
			}
			if( chkNext == TRUE ){
				//���̃`�����l����
				if( sys->chScanChkList.size() <= (size_t)sys->chScanIndexOrStatus + 1 ){
					//�S���`�F�b�N�I������̂ŏI��
					sys->chScanIndexOrStatus = ST_COMPLETE;
					sys->chUtil.SaveChSet(chSet4, chSet5);
					break;
				}
				sys->chScanIndexOrStatus++;
			}
		}
	}

	sys->chUtil.LoadChSet(chSet4, chSet5);
}

//EPG�擾���J�n����
//�߂�l�F
// TRUE�i�����j�AFALSE�i���s�j
//�����F
// chList		[IN]EPG�擾����`�����l���ꗗ(NULL��)
BOOL CBonCtrl::StartEpgCap(
	const vector<SET_CH_INFO>* chList
	)
{
	if( this->tsOut.IsRec() == TRUE ){
		return FALSE;
	}
	if( this->epgCapThread.joinable() && this->epgCapIndexOrStatus >= 0 ){
		return FALSE;
	}

	StopBackgroundEpgCap();
	StopEpgCap();

	if( this->bonUtil.GetOpenBonDriverFileName().empty() == false ){
		this->epgCapIndexOrStatus = ST_COMPLETE;
		if( chList ){
			this->epgCapChList.clear();
			for( size_t i = 0; i < chList->size(); i++ ){
				//SID�w��̂ݑΉ�
				if( (*chList)[i].useSID ){
					this->epgCapChList.push_back((*chList)[i]);
				}
			}
		}else{
			this->epgCapChList = this->chUtil.GetEpgCapService();
		}
		if( this->epgCapChList.empty() ){
			//�擾������̂��Ȃ�
			return TRUE;
		}
		this->epgCapIndexOrStatus = 0;
		//��M�X���b�h�N��
		this->epgCapStopEvent.Reset();
		this->epgCapThread = thread_(EpgCapThread, this);
		return TRUE;
	}

	return FALSE;
}

//EPG�擾���~����
void CBonCtrl::StopEpgCap(
	)
{
	if( this->epgCapThread.joinable() ){
		this->epgCapStopEvent.Set();
		this->epgCapThread.join();
	}
}

//EPG�擾�̃X�e�[�^�X���擾����
//��info==NULL�̏ꍇ�Ɍ���X���b�h�Z�[�t
//�߂�l�F
// �X�e�[�^�X
//�����F
// info			[OUT]�擾���̃T�[�r�X
CBonCtrl::JOB_STATUS CBonCtrl::GetEpgCapStatus(
	SET_CH_INFO* info
	)
{
	int indexOrStatus = this->epgCapIndexOrStatus;
	if( indexOrStatus < 0 ){
		return (JOB_STATUS)indexOrStatus;
	}
	if( info != NULL ){
		*info = this->epgCapChList[indexOrStatus];
	}
	return ST_WORKING;
}

void CBonCtrl::EpgCapThread(CBonCtrl* sys)
{
	BOOL chkNext = TRUE;
	BOOL startCap = FALSE;
	DWORD wait = 0;
	DWORD startTime = 0;

	BOOL chkONIDs[16] = {};

	fs_path iniPath = GetCommonIniPath().replace_filename(L"BonCtrl.ini");

	DWORD timeOut = GetPrivateProfileInt(L"EPGCAP", L"EpgCapTimeOut", 10, iniPath.c_str());
	BOOL saveTimeOut = GetPrivateProfileInt(L"EPGCAP", L"EpgCapSaveTimeOut", 0, iniPath.c_str());

	//Common.ini�͈�ʂɊO���v���Z�X���ύX����\���̂���(�͂���)���̂Ȃ̂ŁA���p�̒��O�Ƀ`�F�b�N����
	fs_path commonIniPath = GetCommonIniPath();
	BOOL basicOnlyONIDs[16] = {};
	basicOnlyONIDs[4] = GetPrivateProfileInt(L"SET", L"BSBasicOnly", 1, commonIniPath.c_str());
	basicOnlyONIDs[6] = GetPrivateProfileInt(L"SET", L"CS1BasicOnly", 1, commonIniPath.c_str());
	basicOnlyONIDs[7] = GetPrivateProfileInt(L"SET", L"CS2BasicOnly", 1, commonIniPath.c_str());
	basicOnlyONIDs[10] = GetPrivateProfileInt(L"SET", L"CS3BasicOnly", 0, commonIniPath.c_str());

	for(;;){
		if( sys->epgCapStopEvent.WaitOne(wait) ){
			//�L�����Z�����ꂽ
			sys->epgCapIndexOrStatus = ST_CANCEL;
			sys->tsOut.StopSaveEPG(FALSE);
			break;
		}
		DWORD chkCount = sys->epgCapIndexOrStatus;
		if( chkNext == TRUE ){
			DWORD space = 0;
			DWORD ch = 0;
			sys->chUtil.GetCh(sys->epgCapChList[chkCount].ONID, sys->epgCapChList[chkCount].TSID,
			                  sys->epgCapChList[chkCount].SID, space, ch);
			sys->ProcessSetCh(space, ch, FALSE, FALSE);
			startTime = GetTickCount();
			chkNext = FALSE;
			startCap = FALSE;
			wait = 1000;
			chkONIDs[min<size_t>(sys->epgCapChList[chkCount].ONID, _countof(chkONIDs) - 1)] = TRUE;
		}else{
			DWORD tick = GetTickCount();
			DWORD elapsed;
			if( sys->tsOut.IsChUnknown(&elapsed) ){
				startTime += min<DWORD>(tick - startTime, 1000);
				if( elapsed > 15000 ){
					//�`�����l���؂�ւ����^�C���A�E�g�����̂Ŗ��M���Ɣ��f
					chkNext = TRUE;
				}
			}else{
				if( tick - startTime > timeOut * 60 * 1000 ){
					//timeOut���ȏォ�����Ă���Ȃ��~
					sys->tsOut.StopSaveEPG(saveTimeOut);
					chkNext = TRUE;
					wait = 0;
					_OutputDebugString(L"++%d����EPG�擾�������� or Ch�ύX�ŃG���[", timeOut);
				}else if( tick - startTime > 5000 ){
					//�؂�ւ���������5�b�ȏ�߂��Ă���̂Ŏ擾����
					if( startCap == FALSE ){
						//�擾�J�n
						startCap = TRUE;
						wstring epgDataPath = L"";
						GetEpgDataFilePath(sys->epgCapChList[chkCount].ONID,
						                   basicOnlyONIDs[min<size_t>(sys->epgCapChList[chkCount].ONID, _countof(basicOnlyONIDs) - 1)] ? 0xFFFF : sys->epgCapChList[chkCount].TSID,
						                   epgDataPath);
						sys->tsOut.StartSaveEPG(epgDataPath);
						wait = 60*1000;
					}else{
						vector<SET_CH_INFO> chkList;
						if( basicOnlyONIDs[min<size_t>(sys->epgCapChList[chkCount].ONID, _countof(basicOnlyONIDs) - 1)] ){
							chkList = sys->chUtil.GetEpgCapServiceAll(sys->epgCapChList[chkCount].ONID);
						}else{
							chkList = sys->chUtil.GetEpgCapServiceAll(sys->epgCapChList[chkCount].ONID, sys->epgCapChList[chkCount].TSID);
						}
						//epgCapChList�̃T�[�r�X��EPG�擾�ΏۂłȂ������Ƃ��Ă��`�F�b�N���Ȃ���΂Ȃ�Ȃ�
						chkList.push_back(sys->epgCapChList[chkCount]);
						for( vector<SET_CH_INFO>::iterator itr = chkList.begin(); itr != chkList.end(); itr++ ){
							if( itr->ONID == chkList.back().ONID && itr->TSID == chkList.back().TSID && itr->SID == chkList.back().SID ){
								chkList.pop_back();
								break;
							}
						}
						//�~�Ϗ�ԃ`�F�b�N
						for( vector<SET_CH_INFO>::iterator itr = chkList.begin(); itr != chkList.end(); itr++ ){
							BOOL leitFlag = sys->chUtil.IsPartial(itr->ONID, itr->TSID, itr->SID);
							pair<EPG_SECTION_STATUS, BOOL> status = sys->tsOut.GetSectionStatusService(itr->ONID, itr->TSID, itr->SID, leitFlag);
							if( status.second == FALSE ){
								status.first = sys->tsOut.GetSectionStatus(leitFlag);
							}
							if( status.first != EpgNoData ){
								chkNext = TRUE;
								if( status.first != EpgHEITAll &&
								    status.first != EpgLEITAll &&
								    (status.first != EpgBasicAll || basicOnlyONIDs[min<size_t>(itr->ONID, _countof(basicOnlyONIDs) - 1)] == FALSE) ){
									chkNext = FALSE;
									break;
								}
							}
						}
						if( chkNext == TRUE ){
							sys->tsOut.StopSaveEPG(TRUE);
							wait = 0;
						}else{
							wait = 10*1000;
						}
					}
				}
			}
			if( chkNext == TRUE ){
				//���̃`�����l����
				chkCount++;
				if( sys->epgCapChList.size() <= chkCount ){
					//�S���`�F�b�N�I������̂ŏI��
					sys->epgCapIndexOrStatus = ST_COMPLETE;
					break;
				}
				//1�`�����l���̂݁H
				if( basicOnlyONIDs[min<size_t>(sys->epgCapChList[chkCount].ONID, _countof(basicOnlyONIDs) - 1)] &&
				    chkONIDs[sys->epgCapChList[chkCount].ONID] ){
					chkCount++;
					while( chkCount < sys->epgCapChList.size() ){
						if( sys->epgCapChList[chkCount].ONID != sys->epgCapChList[chkCount - 1].ONID ){
							break;
						}
						chkCount++;
					}
					if( sys->epgCapChList.size() <= chkCount ){
						//�S���`�F�b�N�I������̂ŏI��
						sys->epgCapIndexOrStatus = ST_COMPLETE;
						break;
					}
				}
				sys->epgCapIndexOrStatus = chkCount;
			}
		}
	}
}

void CBonCtrl::GetEpgDataFilePath(WORD ONID, WORD TSID, wstring& epgDataFilePath)
{
	Format(epgDataFilePath, L"%04X%04X_epg.dat", ONID, TSID);
	epgDataFilePath = GetSettingPath().append(EPG_SAVE_FOLDER).append(epgDataFilePath).native();
}

void CBonCtrl::SaveErrCount(
	DWORD id,
	const wstring& filePath,
	int dropSaveThresh,
	int scrambleSaveThresh,
	ULONGLONG& drop,
	ULONGLONG& scramble
	)
{
	this->tsOut.SaveErrCount(id, filePath, dropSaveThresh, scrambleSaveThresh, drop, scramble);
}

//�^�撆�̃t�@�C���̏o�̓T�C�Y���擾����
//�����F
// id					[IN]���䎯��ID
// writeSize			[OUT]�ۑ��t�@�C����
void CBonCtrl::GetRecWriteSize(
	DWORD id,
	__int64* writeSize
	)
{
	this->tsOut.GetRecWriteSize(id, writeSize);
}

//�o�b�N�O���E���h�ł�EPG�擾�ݒ�
//�����F
// enableLive	[IN]�������Ɏ擾����
// enableRec	[IN]�^�撆�Ɏ擾����
// enableRec	[IN]EPG�擾����`�����l���ꗗ
// *Basic		[IN]�P�`�����l�������{���̂ݎ擾���邩�ǂ���
// backStartWaitSec	[IN]Ch�؂�ւ��A�^��J�n��A�o�b�N�O���E���h�ł�EPG�擾���J�n����܂ł̕b��
void CBonCtrl::SetBackGroundEpgCap(
	BOOL enableLive,
	BOOL enableRec,
	BOOL BSBasic,
	BOOL CS1Basic,
	BOOL CS2Basic,
	BOOL CS3Basic,
	DWORD backStartWaitSec
	)
{
	this->enableLiveEpgCap = enableLive;
	this->enableRecEpgCap = enableRec;
	this->epgCapBackBSBasic = BSBasic;
	this->epgCapBackCS1Basic = CS1Basic;
	this->epgCapBackCS2Basic = CS2Basic;
	this->epgCapBackCS3Basic = CS3Basic;
	this->epgCapBackStartWaitSec = backStartWaitSec;

	StartBackgroundEpgCap();
}

void CBonCtrl::StartBackgroundEpgCap()
{
	StopBackgroundEpgCap();
	if( (this->chScanThread.joinable() == false || this->chScanIndexOrStatus < 0) &&
	    (this->epgCapThread.joinable() == false || this->epgCapIndexOrStatus < 0) ){
		if( this->bonUtil.GetOpenBonDriverFileName().empty() == false ){
			//��M�X���b�h�N��
			this->epgCapBackStopEvent.Reset();
			this->epgCapBackThread = thread_(EpgCapBackThread, this);
		}
	}
}

void CBonCtrl::StopBackgroundEpgCap()
{
	if( this->epgCapBackThread.joinable() ){
		this->epgCapBackStopEvent.Set();
		this->epgCapBackThread.join();
	}
}

void CBonCtrl::EpgCapBackThread(CBonCtrl* sys)
{
	fs_path iniPath = GetCommonIniPath().replace_filename(L"BonCtrl.ini");

	DWORD timeOut = GetPrivateProfileInt(L"EPGCAP", L"EpgCapTimeOut", 10, iniPath.c_str());
	BOOL saveTimeOut = GetPrivateProfileInt(L"EPGCAP", L"EpgCapSaveTimeOut", 0, iniPath.c_str());

	if( sys->epgCapBackStopEvent.WaitOne(sys->epgCapBackStartWaitSec * 1000) ){
		//�L�����Z�����ꂽ
		return;
	}

	if( sys->tsOut.IsRec() == TRUE ){
		if( sys->enableRecEpgCap == FALSE ){
			return;
		}
	}else{
		if( sys->enableLiveEpgCap == FALSE ){
			return;
		}
	}

	DWORD startTime = GetTickCount();

	wstring epgDataPath = L"";
	WORD ONID;
	WORD TSID;
	sys->tsOut.GetStreamID(&ONID, &TSID);

	BOOL basicOnly = ONID == 4 && sys->epgCapBackBSBasic ||
	                 ONID == 6 && sys->epgCapBackCS1Basic ||
	                 ONID == 7 && sys->epgCapBackCS2Basic ||
	                 ONID == 10 && sys->epgCapBackCS3Basic;
	vector<SET_CH_INFO> chkList = sys->chUtil.GetEpgCapServiceAll(ONID, TSID);
	if( chkList.empty() == false && basicOnly ){
		chkList = sys->chUtil.GetEpgCapServiceAll(ONID);
	}
	if( chkList.empty() ){
		return;
	}

	GetEpgDataFilePath(ONID, basicOnly ? 0xFFFF : TSID, epgDataPath);
	sys->tsOut.StartSaveEPG(epgDataPath);

	if( sys->epgCapBackStopEvent.WaitOne(60 * 1000) ){
		//�L�����Z�����ꂽ
		sys->tsOut.StopSaveEPG(FALSE);
		return;
	}
	for(;;){
		//�~�Ϗ�ԃ`�F�b�N
		BOOL chkNext = FALSE;
		for( vector<SET_CH_INFO>::iterator itr = chkList.begin(); itr != chkList.end(); itr++ ){
			BOOL leitFlag = sys->chUtil.IsPartial(itr->ONID, itr->TSID, itr->SID);
			pair<EPG_SECTION_STATUS, BOOL> status = sys->tsOut.GetSectionStatusService(itr->ONID, itr->TSID, itr->SID, leitFlag);
			if( status.second == FALSE ){
				status.first = sys->tsOut.GetSectionStatus(leitFlag);
			}
			if( status.first != EpgNoData ){
				chkNext = TRUE;
				if( status.first != EpgHEITAll &&
				    status.first != EpgLEITAll &&
				    (status.first != EpgBasicAll || basicOnly == FALSE) ){
					chkNext = FALSE;
					break;
				}
			}
		}

		if( chkNext == TRUE ){
			sys->tsOut.StopSaveEPG(TRUE);
			CSendCtrlCmd cmd;
			cmd.SetConnectTimeOut(1000);
			cmd.SendReloadEpg();
			break;
		}else{
			if( GetTickCount() - startTime > timeOut * 60 * 1000 ){
				//timeOut���ȏォ�����Ă���Ȃ��~
				sys->tsOut.StopSaveEPG(saveTimeOut);
				CSendCtrlCmd cmd;
				cmd.SetConnectTimeOut(1000);
				cmd.SendReloadEpg();
				_OutputDebugString(L"++%d����EPG�擾�������� or Ch�ύX�ŃG���[", timeOut);
				break;
			}
		}

		if( sys->epgCapBackStopEvent.WaitOne(10 * 1000) ){
			//�L�����Z�����ꂽ
			sys->tsOut.StopSaveEPG(FALSE);
			break;
		}
	}
}

void CBonCtrl::GetViewStatusInfo(
	float* signalLv,
	int* space,
	int* ch
	)
{
	CBlockLock lock(&this->buffLock);
	*signalLv = this->statusSignalLv;
	*space = this->viewSpace;
	*ch = this->viewCh;
}
