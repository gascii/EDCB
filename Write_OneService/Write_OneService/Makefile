TARGET ?= Write_OneService.dll
CPPFLAGS = -I. -DNDEBUG -DWIN32 -D_WINDOWS -D_UNICODE -DUNICODE
WINDRES ?= windres

$(TARGET): $(addsuffix .obj, \
  BonCtrl__CATUtil.cpp \
  BonCtrl__CreatePATPacket.cpp \
  BonCtrl__PacketInit.cpp \
  BonCtrl__PMTUtil.cpp \
  BonCtrl__ServiceFilter.cpp \
  Common__EpgTimerUtil.cpp \
  Common__PathUtil.cpp \
  Common__StringUtil.cpp \
  Common__TimeUtil.cpp \
  Common__TSBuffUtil.cpp \
  Common__TSPacketUtil.cpp \
  Common__WritePlugInUtil.cpp \
  dllmain.cpp \
  stdafx.cpp \
  WriteMain.cpp \
  Write_OneService.cpp \
  )
	$(CXX) -Wl,-s -Wl,--kill-at -Wl,--dynamicbase -Wl,--nxcompat -static-libgcc -static-libstdc++ -shared -o $@ $^
BonCtrl__%.cpp.obj: ../../BonCtrl/%.cpp
	$(CXX) -c $(CPPFLAGS) -finput-charset=CP932 -Os -o $@ $<
Common__%.cpp.obj: ../../Common/%.cpp
	$(CXX) -c $(CPPFLAGS) -finput-charset=CP932 -Os -o $@ $<
%.cpp.obj: %.cpp
	$(CXX) -c $(CPPFLAGS) -finput-charset=CP932 -Os -o $@ $<
%.rc.obj: %.rc
	iconv -f UTF-16LE -t UTF-8 $< | $(WINDRES) $(CPPFLAGS) -c 65001 -J rc -O coff -o $@
clean:
	$(RM) *.obj $(TARGET)
