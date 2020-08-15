using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace DownUnder.RuntimeContent
{
    public static class RuntimeLoader
    {
        // https://community.monogame.net/t/solved-is-it-possible-to-directly-load-a-effect-from-a-class-files-string/10486/8
        public static Effect CompileFX(GraphicsDevice gd, string sourceFile)
        {
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
