#region Comment Head


#endregion

using Sirenix.OdinInspector;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    public class InjectorExample : MonoBehaviour
    {
        private AppContext appContext;
        
        [Button("AppA","应用A",
            ButtonSizes.Medium)]
        public void AppA()
        {
            var injectorBuilder = new InjectorBuilder();
            appContext = new AppContext(Debug.Log,injectorBuilder);
            injectorBuilder.Mapping<ICar,BMW>();
            ReflectionUtility.InvokeMethod(appContext, "Start",
                new object[] {null});
            
            DriveCar();
        }

        private void DriveCar()
        {
            appContext.Get<ICar>(true).Drive();
        }

        [Button("AppB","应用B",
            ButtonSizes.Medium)]
        private void AppB()
        {
            var injectorBuilder = new InjectorBuilder();
            appContext = new AppContext(Debug.Log,injectorBuilder);
            injectorBuilder.Mapping<ICar,Benz>();
            ReflectionUtility.InvokeMethod(appContext, "Start",
                new object[] {null});
            
            DriveCar();
        }

        [Button("Dependency chain","依赖链",
            ButtonSizes.Medium)]
        private void DependencyChain()
        {
            var injectorBuilder = new InjectorBuilder();
            appContext = new AppContext(Debug.Log,injectorBuilder);
            ReflectionUtility.InvokeMethod(appContext, "Start",
                new object[] {null});

            var phone = appContext.Get<Phone>(true);
            phone.Charging();
        }
    }
}
