#pragma once
#include "../../Common/ThreadUtil.h"
#include "NotifyManager.h"
#include <functional>

class CBatManager
{
public:
	struct BAT_WORK_INFO {
		wstring batFilePath;
		vector<pair<string, wstring>> macroList;
	};
	CBatManager(CNotifyManager& notifyManager_, LPCWSTR tmpBatFileName);
	~CBatManager();

	void Finalize();
	void AddBatWork(const BAT_WORK_INFO& info);
	void SetIdleMargin(DWORD marginSec);
	void SetCustomHandler(LPCWSTR ext, const std::function<void(BAT_WORK_INFO&, vector<char>&)>& handler);

	bool IsWorking() const;
	bool IsWorkingWithoutNotification() const;
protected:
	mutable recursive_mutex_ managerLock;

	CNotifyManager& notifyManager;
	wstring tmpBatFilePath;

	vector<BAT_WORK_INFO> workList;
	std::function<void(BAT_WORK_INFO&, vector<char>&)> customHandler;
	wstring customExt;
	DWORD idleMargin;
	DWORD nextBatMargin;
	bool batWorkExitingFlag;
	thread_ batWorkThread;
	CAutoResetEvent batWorkEvent;
	atomic_bool_ batWorkStopFlag;
protected:
	void StartWork();
	static void BatWorkThread(CBatManager* sys);

	bool CreateBatFile(BAT_WORK_INFO& info, DWORD& exBatMargin, DWORD& exNotifyInterval, WORD& exSW, wstring& exDirect, vector<char>& buff) const;
	static bool ExpandMacro(const string& var, const BAT_WORK_INFO& info, wstring& strWrite);
	static wstring CreateEnvironment(const BAT_WORK_INFO& info);
};

