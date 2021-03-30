using CSScriptLib;
using System.IO;
using System;
using DisasterEngine;
namespace Disaster
{
    class ScriptRunner
    {
        dynamic script;
        IEvaluator evaluator;
        public ScriptRunner()
        {
            //new CSharpCodeProvider();
            evaluator = CSScript.Evaluator;
            evaluator.Reset(false);
            evaluator.ReferenceAssemblyByNamespace("DisasterEngine")
                .ReferenceAssemblyOf(this);

            try
            {
                script = evaluator.LoadCode(
                    File.ReadAllText(Assets.LoadPath("main.cs"))
                );
                script.Init();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void Update(float deltaTime)
        {
            if (script != null)
            {
                script.Update(deltaTime);
            }
        }

    }
}
