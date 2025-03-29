using System;
using Game.UI;

namespace Game.SceneManagement
{
    internal class LoadingUIReporter : IProgress<float>
    {
        private readonly LoadingScreenUI _ui;

        public LoadingUIReporter(LoadingScreenUI ui) => _ui = ui;
        public void Report(float value) => _ui.UpdateProgress(value);
    }
}
