using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DownUnder.RuntimeContent.Effects;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace DownUnder.RuntimeContent
{
    internal static class RuntimeLoader
    {
        private static string _TEMP_FILE_NAME = "downunder_temp_effect_file";

        internal static Effect CompileFX(GraphicsDevice gd, IEffectSource source)
        {
            string sourceFile = Directory.GetCurrentDirectory() + "/" + _TEMP_FILE_NAME + ".txt";
            File.WriteAllText(sourceFile, source.FX);
            Effect result = CompileFX(gd, sourceFile);
            File.Delete(sourceFile);
            return result;
        }

        // https://community.monogame.net/t/solved-is-it-possible-to-directly-load-a-effect-from-a-class-files-string/10486/8
        public static Effect CompileFX(GraphicsDevice gd, string sourceFile)
        {
            return new BasicEffect(gd);
            EffectImporter importer = new EffectImporter();
            EffectContent content = importer.Import(sourceFile, null);
            EffectProcessor processor = new EffectProcessor();
            PipelineManager pm = new PipelineManager(string.Empty, string.Empty, string.Empty);
            PipelineProcessorContext ppc = new PipelineProcessorContext(pm, new PipelineBuildEvent());
            CompiledEffectContent cecontent = processor.Process(content, ppc);
            ContentCompiler compiler = new ContentCompiler();

            return new Effect(gd, cecontent.GetEffectCode());
        }
    }
}
