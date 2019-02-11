#pragma once


#include "../../BonCtrl/BonCtrlDef.h"

// CSetDlgNetwork �_�C�A���O

class CSetDlgNetwork
{
public:
	CSetDlgNetwork();   // �W���R���X�g���N�^�[
	~CSetDlgNetwork();
	BOOL Create(LPCWSTR lpszTemplateName, HWND hWndParent);
	HWND GetSafeHwnd() const{ return m_hWnd; }
	void SaveIni(void);

// �_�C�A���O �f�[�^
	enum { IDD = IDD_DIALOG_SET_NW };

protected:
	HWND m_hWnd;
	vector<NW_SEND_INFO> udpSendList;
	vector<NW_SEND_INFO> tcpSendList;

	BOOL OnInitDialog();
	void OnBnClickedButtonAddUdp();
	void OnBnClickedButtonDelUdp();
	void OnBnClickedButtonAddTcp();
	void OnBnClickedButtonDelTcp();
	void OnBnClickedRadioTcp();
	static INT_PTR CALLBACK DlgProc(HWND hDlg, UINT uMsg, WPARAM wParam, LPARAM lParam);
	HWND GetDlgItem(int nID) const{ return ::GetDlgItem(m_hWnd, nID); }
};
