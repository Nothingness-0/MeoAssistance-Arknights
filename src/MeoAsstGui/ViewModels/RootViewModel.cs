using System.Threading.Tasks;
using Stylet;
using StyletIoC;

namespace MeoAsstGui
{
    public class RootViewModel : Conductor<Screen>.Collection.OneActive
    {
        private IContainer _container;
        private IWindowManager _windowManager;

        public RootViewModel(IContainer container, IWindowManager windowManager)
        {
            _container = container;
            _windowManager = windowManager;
        }

        protected override void OnViewLoaded()
        {
            // 有个界面一启动就崩溃的问题，暂时找不到原因，而且日志也没生成
            // 因为下一行就有日志生成了，所以可能是这里崩的
            // 反正先try起来看看有没有用_(:з」∠)_
            try
            {
                CheckAndUpdateNow();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("CheckAndUpdateNow Exception caught: {0}", e);
            }
            InitProxy();
            InitViewModels();
            ShowUpdateOrDownload();
        }

        private void InitProxy()
        {
            var p = _container.Get<AsstProxy>();
            p.Init();
        }

        private void InitViewModels()
        {
            var mfvm = _container.Get<MainFunctionViewModel>();
            var rvm = _container.Get<RecruitViewModel>();
            var ivm = _container.Get<InfrastructureConstructionViewModel>();
            var tvm = _container.Get<TaskQueueViewModel>();

            Items.Add(tvm);
            Items.Add(mfvm);
            Items.Add(ivm);
            Items.Add(rvm);
            ActiveItem = tvm;
        }

        private bool CheckAndUpdateNow()
        {
            var vuvm = _container.Get<VersionUpdateViewModel>();
            return vuvm.CheckAndUpdateNow();
        }

        private async void ShowUpdateOrDownload()
        {
            var vuvm = _container.Get<VersionUpdateViewModel>();

            if (vuvm.IsFirstBootAfterUpdate)
            {
                _windowManager.ShowWindow(vuvm);
            }
            else
            {
                var task = Task.Run(() =>
                {
                    return vuvm.CheckAndDownloadUpdate();
                });

                await task;
            }
        }
    }
}
