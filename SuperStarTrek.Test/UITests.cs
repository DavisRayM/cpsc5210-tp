using System.Diagnostics;
using System.Text;
using SuperStarTrek.Resources;
using System.Text.RegularExpressions;

namespace SuperStarTrek.Test.Systems
{
    public class UITests
    {
        Process? process;
        StringBuilder? output;
        StreamWriter? writer;
        StreamReader? reader;
        int writeCount = 0;

        [SetUp]
        public void GameProcess()
        {
            this.process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "../SuperStarTrek",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            this.output = new StringBuilder();
            this.writeCount = 0;
            this.process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    this.output.AppendLine(args.Data);
                    printOutput();
                    writeToFile(TestContext.CurrentContext.Test.Name, this.writeCount);
                    this.writeCount += 1;
                }
            };
            this.process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    TestContext.WriteLine(args.Data);
                }
            };

            this.process.Start();
            this.process.BeginOutputReadLine();
            this.process.BeginErrorReadLine();
            this.writer = this.process.StandardInput;
            this.writer.AutoFlush = true;
        }

        [TearDown]
        public void TearDown_Process()
        {
            this.process.Dispose();
        }

        public void RequestProcessShutdown()
        {
            this.writer.WriteLine("XXX");
            this.writer.WriteLine("Nay");
            this.writer.Close();
            this.process.WaitForExit();
        }

        public void writeToFile(String name, int uniqueID)
        {
            string fileName = $"test-{name}-{uniqueID}.txt";
            File.WriteAllText(SanitizeFileName(fileName), this.output.ToString());
        }

        public static string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            string safe = new string(name.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
            safe = Regex.Replace(safe, "_{2,}", "_");
            return safe.Trim('_', ' ');
        }

        public void printOutput()
        {
            TestContext.WriteLine($"Test Title: {TestContext.CurrentContext.Test.Name}");
            TestContext.WriteLine(this.output);
            TestContext.WriteLine("====================================================");
        }

        [Test]
        public void Game_PromptsTutorial_OnOpen()
        {
            writer.WriteLine("N");
            RequestProcessShutdown();
            string output = this.output.ToString();
            Assert.True(output.Contains("Do you need instructions (Y/N)?"));
        }

        [Test]
        public void Game_ShowsTutorial_IfAccepted()
        {
            writer.WriteLine("Y");
            RequestProcessShutdown();
            string output = this.output.ToString();
            Assert.True(output.Contains(Strings.Instructions));
        }

        [Test]
        public void Game_Title_Visible()
        {
            writer.WriteLine("N");
            RequestProcessShutdown();
            string output = this.output.ToString();
            Assert.True(output.Contains(Strings.Title));
        }

        [Test]
        [TestCase("Stardate")]
        [TestCase("Condition")]
        [TestCase("Quadrant")]
        [TestCase("Sector")]
        [TestCase("Photon torpedoes")]
        [TestCase("Shields")]
        [TestCase("Klingons remaining")]
        [TestCase("Your orders are as follows")]
        public void Game_UIElements_ParametizedTest(string expected)
        {
            printOutput();
            writer.WriteLine("N");
            RequestProcessShutdown();
            string output = this.output.ToString();
            Assert.True(output.Contains(expected));
        }

        [Test]
        public void Game_NavCommand_CorrectVisual()
        {
            printOutput();
            writer.WriteLine("N");
            writer.WriteLine("NAV");
            writer.WriteLine("1");
            writer.WriteLine("2");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains("Course (1-9)? Warp Factor (0-8)?"));
        }

        [Test]
        public void Game_ShieldCommand_RaisesShields()
        {
            writer.WriteLine("N");
            writer.WriteLine("SHE");
            writer.WriteLine("10");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains("Shields now at 10 units per your command."));
        }

        [Test]
        public void Game_ShieldCommand_RequestShieldAmount()
        {
            writer.WriteLine("N");
            writer.WriteLine("SHE");
            writer.WriteLine("10");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains("Number of units to shields?"));
        }

        [Test]
        public void Game_DamageReport_ShowCasesDamageInfo()
        {
            writer.WriteLine("N");
            writer.WriteLine("DAM");
            RequestProcessShutdown();

            string output = this.output.ToString();
            string expected = @"Device             State of Repair
Warp Engines              0 
Short Range Sensors       0 
Long Range Sensors        0 
Phaser Control            0 
Photon Tubes              0 
Shield Control            0 
Damage Control            0 
Library-Computer          0";
            Assert.True(output.Contains(expected));
        }

        [Test]
        public void Game_ComputerCommand_RequestsOption()
        {
            writer.WriteLine("N");
            writer.WriteLine("COM");
            writer.WriteLine("10");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains("Computer active and waiting command?"));
        }

        [Test]
        [TestCase("!Number expected - retry input line")]
        [TestCase("0 = Cumulative galactic record")]
        [TestCase("1 = Status report")]
        [TestCase("2 = Photon torpedo data")]
        [TestCase("3 = Starbase nav data")]
        [TestCase("4 = Direction/distance calculator")]
        [TestCase("5 = Galaxy 'region name' map")]
        public void Game_ComputerCommand_WrongInputShowsOptions(string expected)
        {
            writer.WriteLine("N");
            writer.WriteLine("COM");
            writer.WriteLine("10");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains(expected));
        }

        [Test]
        [TestCase("Photon torpedo course (1-9)?")]
        [TestCase("Torpedo track:")]
        public void Game_TorpedoCommand(string expected)
        {
            writer.WriteLine("N");
            writer.WriteLine("TOR");
            writer.WriteLine("1");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains(expected));
        }

        [Test]
        [TestCase("NAV  (To set course)")]
        [TestCase("SRS  (For short range sensor scan)")]
        [TestCase("LRS  (For long range sensor scan)")]
        [TestCase("PHA  (To fire phasers)")]
        [TestCase("TOR  (To fire photon torpedoes)")]
        [TestCase("SHE  (To raise or lower shields)")]
        [TestCase("DAM  (For damage control reports)")]
        [TestCase("COM  (To call on library-computer)")]
        [TestCase("XXX  (To resign your command)")]
        public void Game_HelpCommand_PrintsAvailableCommands(string expected)
        {
            writer.WriteLine("N");
            writer.WriteLine("?");
            RequestProcessShutdown();

            string output = this.output.ToString();
            Assert.True(output.Contains(expected));
        }
    }
}
