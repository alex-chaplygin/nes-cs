src=Memory.cs Program.cs CPU.cs Cartridge.cs Mapper.cs PPU.cs Input.cs Mappers/MMC1.cs Mappers/MMC2.cs APU/Divider.cs APU/APU.cs

all: /tmp/Program.exe

/tmp/Program.exe:	$(src) Program.cs
	mcs $^ -out:$@ -r:System.Drawing.dll
