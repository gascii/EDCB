
// stdafx.h : �W���̃V�X�e�� �C���N���[�h �t�@�C���̃C���N���[�h �t�@�C���A�܂���
// �Q�Ɖ񐔂������A�����܂�ύX����Ȃ��A�v���W�F�N�g��p�̃C���N���[�h �t�@�C��
// ���L�q���܂��B

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Windows �w�b�_�[����g�p����Ă��Ȃ����������O���܂��B
#define NOMINMAX
// Windows �w�b�_�[ �t�@�C��:
#include <windows.h>
#include <windowsx.h>
#include <commctrl.h>
#include <shellapi.h>
#include <commdlg.h>
#include <shlobj.h>
#pragma comment(lib, "comctl32.lib")
#pragma comment(lib, "ws2_32.lib")

// C �����^�C�� �w�b�_�[ �t�@�C��
#include <stdio.h>

#define afx_msg

#if defined(_UNICODE) && defined(OutputDebugString)
#undef OutputDebugString
#define OutputDebugString OutputDebugStringWrapper
// OutputDebugStringW�̃��b�p�[�֐�
// API�t�b�N�ɂ�鍂�x�Ȃ��̂łȂ��P�Ȃ�u���BOutputDebugStringA��DLL����̌Ăяo���̓��b�v����Ȃ�
void OutputDebugStringWrapper(LPCWSTR lpOutputString);
#endif
void SetSaveDebugLog(bool saveDebugLog);

#include "../../Common/Common.h"
