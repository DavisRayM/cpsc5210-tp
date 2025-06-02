using System.Diagnostics;
using SuperStarTrek.Resources;

namespace SuperStarTrek.Test.Systems
{
    public class GameTests
    {
        public Process GameProcess()
        {
            var process = new Process
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

            return process;
        }

        [Test]
        public void Game_PromptsTutorial_OnOpen()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains("Do you need instructions (Y/N)?"));
        }

        [Test]
        public void Game_ShowsTutorial_IfAccepted()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("Y");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains(Strings.Instructions));
        }

        [Test]
        public void Game_Title_Visible()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
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
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.Flush();
            writer.WriteLine("XXX");
            writer.Flush();
            writer.WriteLine("Nay");
            writer.Flush();
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains(expected));
        }

        [Test]
        public void Game_NavCommand_CorrectVisual()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("NAV");
            writer.WriteLine("1");
            writer.WriteLine("2");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains("Course (1-9)? Warp Factor (0-8)?"));
        }

        [Test]
        public void Game_ShieldCommand_RaisesShields()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("SHE");
            writer.WriteLine("10");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains("Shields now at 10 units per your command."));
        }

        [Test]
        public void Game_DamageReport_ShowCasesDamageInfo()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("DAM");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            string expected = @"Device             State of Repair
Warp Engines              0 
Short Range Sensors       0 
Long Range Sensors        0 
Phaser Control            0 
Photon Tubes              0 
Shield Control            0 
Damage Control            0 
Library-Computer          0";
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains(expected));
        }

        [Test]
        public void Game_ComputerCommand_RequestsOption()
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("COM");
            writer.WriteLine("10");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
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
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("COM");
            writer.WriteLine("10");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
            Assert.True(output.Contains(expected));
        }

        [Test]
        [TestCase("Photon torpedo course (1-9)?")]
        [TestCase("Torpedo track:")]
        public void Game_TorpedoCommand(string expected)
        {
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("TOR");
            writer.WriteLine("1");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine(output);
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
            var process = GameProcess();
            process.Start();
            var writer = process.StandardInput;
            var reader = process.StandardOutput;

            writer.WriteLine("N");
            writer.WriteLine("?");
            writer.WriteLine("XXX");
            writer.WriteLine("Nay");
            writer.Close();

            string output = reader.ReadToEnd();
            process.WaitForExit();
            TestContext.WriteLine("-------");
            TestContext.WriteLine(output);
            Assert.True(output.Contains(expected));
        }
    }
}
