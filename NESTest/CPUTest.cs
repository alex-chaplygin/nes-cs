using NES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NESTest
{
    [TestClass]
    public class CPUTest
    {
        [TestMethod]
        public void StackTest()
        {
            CPU.SP = 0xFF;
            CPU.Push(0x11);
            Assert.AreEqual(0x11, Memory.memory[0x1FF]);
            Assert.AreEqual(0xFE, CPU.SP);
        }

        [TestMethod]
        public void StackTest2()
        {
            CPU.SP = 0xFF;
            CPU.Push(0x11);
            Assert.AreEqual(0x11, CPU.Pop());
            Assert.AreEqual(0xFF, CPU.SP);
        }

        [TestMethod]
        public void StackTest3()
        {
            CPU.SP = 0xFF;
            CPU.PushWord(0x1234);
            //CPU.Push(0x34); CPU.Push(0x12);
            Assert.AreEqual(0x34, Memory.memory[0x1FF]);
            Assert.AreEqual(0x12, Memory.memory[0x1FE]);
            Assert.AreEqual(0x1234, CPU.ToWord(Memory.memory[0x1FE], Memory.memory[0x1FF]));
            Assert.AreEqual(0xFD, CPU.SP);
        }

        [TestMethod]
        public void StackTest4()
        {
            CPU.SP = 0xFD;
            Assert.AreEqual(CPU.PopWord(), CPU.ToWord(Memory.memory[0x1FE], Memory.memory[0x1FF]));
            Assert.AreEqual(0xFF, CPU.SP);
        }

        [TestMethod]
        public void InterruptTest()
        {
            CPU.PC = 0x1234;
            CPU.SP = 0x80;
            CPU.interrupt_flag = true;
            CPU.Interrupt(Interruption.IRQ);
            Assert.AreEqual(true, CPU.interrupt_flag);
            Assert.AreEqual(0x80, CPU.SP);
            Assert.AreEqual(0, CPU.PopWord());
            Assert.AreEqual(0x1234, CPU.PC);
        }

        [TestMethod]
        public void InterruptTest2()
        {
            CPU.PC = 0x1234;
            CPU.SP = 0x80;
            CPU.interrupt_flag = true;
            CPU.Interrupt(Interruption.NMI);
            Assert.AreEqual(true, CPU.interrupt_flag);
            Assert.AreEqual(0x80 - 3, CPU.SP);
            Assert.AreEqual(0x1234, CPU.PopWord());
            ushort pc = Memory.ReadWord(0xFFFA);
            Assert.AreEqual(pc, CPU.PC);
        }
        [TestMethod]
        public void InterruptTest3()
        {
            CPU.SP = 0x90; CPU.interrupt_flag = true; CPU.PC = 0x1234;
            CPU.Interrupt(Interruption.RESET);
            Assert.AreEqual(true, CPU.interrupt_flag);
            Assert.AreEqual(0xFF, CPU.SP); CPU.SP = 0x90 - 3;
            Assert.AreEqual(0x1234, CPU.PopWord());
            ushort pc = Memory.ReadWord(0xFFFC);
            Assert.AreEqual(pc, CPU.PC);
        }

        [TestMethod]
        public void InterruptTest4()
        {
            CPU.interrupt_flag = false; CPU.SP = 0xFF; CPU.PC = 0x1234;
            CPU.Interrupt(Interruption.IRQ);
            Assert.AreEqual(false, CPU.interrupt_flag);
            Assert.AreEqual(0xFF - 3, CPU.SP);
            Assert.AreEqual(0x1234, CPU.PopWord());
            ushort pc = Memory.ReadWord(0xFFFE);
            Assert.AreEqual(pc, CPU.PC);

        }
        [TestMethod]
        public void StepTestBRK()//BRK == IRQ
        {
            CPU.SP = 0xFF;
            CPU.PC = 0x234; byte instr = 0;// 0 соотв.номеру BRK в CPU.table
            CPU.interrupt_flag = false;
            Memory.memory[CPU.PC] = instr;
            //Вектор прерывания
            Memory.memory[0xFFFE] = 0xCD;
            Memory.memory[0xFFFF] = 0xAB;
            int cycles = CPU.Step();
            Assert.AreEqual(7, cycles);
            Assert.AreEqual(0xABCD, CPU.PC);
            Assert.AreEqual(0x235, CPU.PopWord());
        }

        //Киселев Николай
        //       [TestMethod]
        //       public void AbsoluteTest()
        //       {
        //           CPU.PC = 0x2253;
        //           CPU.Y = 8;

        //           Memory.Write(CPU.PC, 0x27);
        //           Memory.Write((ushort)(CPU.PC+1), 0x17);
        //           Memory.Write((ushort)(0x1727 + CPU.Y), 35);
        //           ushort adr = 0; // = 0x2253; //Так как  CPU.AbsoluteY обязательно требует аргумент, то допустим он такой. Я так понял
        //                                        //это не имеет значение, ведь  внутри функции изменяется значение этого аргумента

        //           Assert.AreEqual(Memory.Read((ushort)(0x1727 + CPU.Y)), CPU.AbsoluteY(ref adr)); //Проверка на совпадение значений
        //           Assert.AreEqual(0x1727 + CPU.Y, adr); // Проверка на совпадение адреса
        //       }

        //[TestMethod]
        //       public void ASL_Test_1()
        //       {
        //           byte val = 0xFF;
        //           ushort adr = 0;
        //           CPU.ASL(adr);
        //           Assert.AreEqual(true, CPU.carry_flag);
        //           Assert.AreEqual(254, CPU.A);
        //           Assert.AreEqual(false, CPU.zero_flag);
        //           Assert.AreEqual(true, CPU.negative_flag);
        //       }

        //       [TestMethod]
        //       public void ASL_Test_2()
        //       {
        //           byte val = 0x00;
        //           ushort adr = 0;
        //           CPU.ASL(val, adr);
        //           Assert.AreEqual(false, CPU.carry_flag);
        //           Assert.AreEqual(0, CPU.A);
        //           Assert.AreEqual(true, CPU.zero_flag);
        //           Assert.AreEqual(false, CPU.negative_flag);
        //       }

        //[TestMethod]
        //       public void ADCTest()
        //       {
        //           ushort adr = 0x12;
        //           CPU.A = 0;
        //           CPU.ADC(0x05, adr);
        //           Assert.AreEqual(5, CPU.A);

        //           CPU.carry_flag = true;
        //           CPU.ADC(0x03, adr);
        //           Assert.AreEqual(0x09, CPU.A);

        //           CPU.carry_flag = false;
        //           CPU.ADC(0xFF, adr);
        //           Assert.AreEqual(0x08, CPU.A);
        //           Assert.IsTrue(CPU.carry_flag);

        //           CPU.A = 0;
        //           CPU.ADC(0x80, adr);
        //           Assert.IsTrue(!CPU.overflow_flag);

        //           CPU.A = 0;
        //           CPU.ADC(0x0F, adr);
        //           Assert.IsTrue(!CPU.overflow_flag);

        //           CPU.A = 0x80;
        //           CPU.ADC(0x80, adr);
        //           Assert.IsTrue(CPU.overflow_flag);

        //           CPU.A = 64;
        //           CPU.ADC(64, adr);
        //           Assert.IsTrue(CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void SBCTest()
        //       {
        //           ushort adr = 0x12;
        //           CPU.A = 0xFF;
        //           CPU.SBC(0x0F, adr);
        //           Assert.AreEqual(0xEF, CPU.A);

        //           CPU.carry_flag = true;
        //           CPU.A = 0xFF;
        //           CPU.SBC(0x0F, adr);
        //           Assert.AreEqual(0xF0, CPU.A);

        //           CPU.carry_flag = false;
        //           CPU.A = 0x00;
        //           CPU.SBC(0x01, adr);
        //           Assert.AreEqual(0xFE, CPU.A);
        //           Assert.IsTrue(CPU.carry_flag);

        //           CPU.A = 0;
        //           CPU.SBC(0x80, adr);
        //           Assert.IsTrue(!CPU.overflow_flag);

        //           CPU.A = 0xFF;
        //           CPU.SBC(0x00, adr);
        //           Assert.IsTrue(!CPU.overflow_flag);

        //           CPU.A = 0x80;
        //           CPU.carry_flag = true;
        //           CPU.SBC(0x80, adr);
        //           Assert.IsTrue(CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void XIndirectTest()
        //       {
        //           CPU.X = 0x4;
        //           CPU.PC = 1;
        //           Memory.memory[1] = 0x10;
        //           Memory.memory[0x14] = 0x00;
        //           Memory.memory[0x15] = 0x82;
        //           Memory.memory[0x8200] = 0xAA;
        //           ushort adr = 0;
        //           byte val = CPU.XIndirect(ref adr);
        //           Assert.AreEqual(0xAA, val);
        //           Assert.AreEqual(0x8200, adr);
        //       }

        //       [TestMethod]
        //       public void XIndirectWrappingTest()
        //       {
        //           CPU.X = 0x15;

        //           CPU.PC = 1;
        //           Memory.memory[1] = 0xFF;
        //           Memory.memory[0x14] = 0x00;
        //           Memory.memory[0x15] = 0x82;
        //           Memory.memory[0x8200] = 0xAA;

        //           ushort adr = 0xA0;
        //           byte val = CPU.XIndirect(ref adr);
        //           Assert.AreEqual(0xAA, val);
        //           Assert.AreEqual(0x8200, adr);
        //       }

        //[TestMethod]
        //      public void BITTest1()
        //       {
        //           CPU.A = 0xFF;
        //           CPU.BIT(0x11, 0);
        //           Assert.AreEqual(false, CPU.zero_flag);
        //           Assert.AreEqual(false, CPU.negative_flag);
        //           Assert.AreEqual(false, CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void BITTest2()
        //       {
        //           CPU.A = 0xFF;
        //           CPU.BIT(0x00, 0);
        //           Assert.AreEqual(true, CPU.zero_flag);
        //           Assert.AreEqual(false, CPU.negative_flag);
        //           Assert.AreEqual(false, CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void BITTest3()
        //       {
        //           CPU.A = 0xFF;
        //           CPU.BIT(0x88, 0);
        //           Assert.AreEqual(false, CPU.zero_flag);
        //           Assert.AreEqual(true, CPU.negative_flag);
        //           Assert.AreEqual(false, CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void BITTest4()
        //       {
        //           CPU.A = 0xFF;
        //           CPU.BIT(0x44, 0);
        //           Assert.AreEqual(false, CPU.zero_flag);
        //           Assert.AreEqual(false, CPU.negative_flag);
        //           Assert.AreEqual(true, CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void BITTest5()
        //       {
        //           CPU.A = 0x00;
        //           CPU.BIT(0x11, 0);
        //           Assert.AreEqual(true, CPU.zero_flag);
        //           Assert.AreEqual(false, CPU.negative_flag);
        //           Assert.AreEqual(false, CPU.overflow_flag);
        //       }

        //       [TestMethod]
        //       public void BITTest6()
        //       {
        //           CPU.A = 0xFF;
        //           CPU.BIT(0xC0, 0);
        //           Assert.AreEqual(false, CPU.zero_flag);
        //           Assert.AreEqual(true, CPU.negative_flag);
        //           Assert.AreEqual(true, CPU.overflow_flag);
        //       }
    }
}
