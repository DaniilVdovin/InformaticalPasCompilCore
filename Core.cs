using System;
using System.Collections.Generic;
using System.Threading;

namespace Informatical
{

    internal class Core
    {
        string 
            root_path,
            pas_path,
            exe_path,
            models_path,
            fpc;
        public string LOG;
        public string[] models = new string[]
        {
            @"\sum.py"
        };

        public event EventHandler ThresholdReached;

        protected virtual void OnUpdateLog(EventArgs e)
        {
            EventHandler handler = ThresholdReached;
            handler?.Invoke(this, e);
        }

        System.Diagnostics.Process process;
        System.Diagnostics.ProcessStartInfo startInfo;

        public Core()
        {
            root_path = $@"{AppDomain.CurrentDomain.BaseDirectory}";

            models_path = $@"{root_path}models";
            fpc = $@"{root_path}FPC\bin\i386-win32\fpc.exe";
   

            process = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = $@"Cmd.exe";

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;

            getModules();


        }
        public void printModules()
        {
            LOG += "MODELS:\n";
            foreach (string it in models) LOG += $@"* {it}"+"\n";
            LOG += "END\n";
        }
        public void getModules()
        {
            List<string> mod = new List<string>();
            string tree_models = RunPromt($@"/C tree /F {models_path}");
            var sta = tree_models.Split('\n');
            bool detect = false;
            foreach (string it in sta)
            {
                if (it.Contains("®"))
                    detect = false;

                if (detect)
                {
                    if(it.Trim()!= "")
                    {
                                            LOG += it.Trim();
                    mod.Add($@"\{it.Trim()}");
                    }

                }
                    

                if (it.Contains(models_path.ToUpper()))
                    detect = true;

            }
            models = mod.ToArray();
            ThresholdReached?.Invoke(this, EventArgs.Empty);
        }
        public string CompileFilePascal(string file_path)
        {
            LOG += RunPromt($@"/C {fpc} {file_path}");
            ThresholdReached?.Invoke(this, EventArgs.Empty);
            return exe_path;
        }
        public string[] TestWithModyle(string models, int iteration)
        {
            List<string> bools = new List<string>();
            string temp_path = models_path + models;

            Random random = new Random();
            int
                min_random = 0,
                max_random = 0,
                index_args = 0;
            string temps = RunPromt($"/C {temp_path} -r");
            string[] Rules = temps.Split(':');
            LOG+= "RULES: "+Rules[0]+"\n";
            ThresholdReached?.Invoke(this, EventArgs.Empty);
            if (Rules[0] == "r")
            {
                min_random = Convert.ToInt32(Rules[1]);
                max_random = Convert.ToInt32(Rules[2]);
                index_args = Convert.ToInt32(Rules[3]);
            }

            for (int i = 0; i < iteration; i++)
            {
                var arg = $@"";
                string[] arg_string_array = new string[index_args];
                if (Rules[0] == "r")
                {
                    for (int j = 0; j < index_args; j++)
                    {
                        arg_string_array[j] = random.Next(min_random, max_random).ToString();
                        arg += $@" {arg_string_array[j]}";
                    }
                }
                var answer = RunPromt($@"/C {temp_path + arg}");
                var cur_answer = RunPromt($@"/C {exe_path}", arg_string_array);
               

                if (Convert.ToInt32(answer) == Convert.ToInt32(cur_answer))
                    bools.Add($@"{i}:{answer}=={cur_answer}:{arg}");
                LOG+= $@"{answer}={cur_answer}"+"\n";
            }
            ThresholdReached?.Invoke(this, EventArgs.Empty);
            return bools.ToArray();
        }
        public string RunPromt(string cmd, params string[] args)
        {
            string st = $"";

            startInfo.Arguments = cmd;

            process.OutputDataReceived += ((s, e) =>
            {   
                if(e.Data!= null && e.Data.Contains("Linking"))
                {
                   exe_path = e.Data.Split(' ')[1];
                }    
                st += (e.Data + "\n");
            });

            process.Start();

            process.BeginOutputReadLine();

            foreach (string a in args) process.StandardInput.WriteLine(a);

            process.WaitForExit(500);
            
            process.CancelOutputRead();

            return st;
        }
    }
}