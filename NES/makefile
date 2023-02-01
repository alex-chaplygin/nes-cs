src=Memory.cs Program.cs CPU.cs Cartridge.cs Mapper.cs PPU.cs Input.cs Mappers/MMC1.cs Mappers/MMC2.cs Mappers/MMC3.cs APU/Divider.cs APU/APU.cs APU/DMC.cs APU/LengthCounter.cs APU/Sequencer.cs APU/Sweep.cs APU/Envelope.cs APU/Noise.cs APU/Pulse.cs

all: /tmp/Program.exe

/tmp/Program.exe:	$(src) Program.cs
	mcs $^ -out:$@ -r:System.Drawing.dll
