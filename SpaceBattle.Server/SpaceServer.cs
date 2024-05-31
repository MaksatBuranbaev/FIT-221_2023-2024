using System;
using System.Diagnostics.CodeAnalysis;
using CoreWCF;
using Hwdtech;

namespace SpaceBattle.Server
{
    [ExcludeFromCodeCoverage]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class SpaceServer : ISpaceServer
    {
        public string Order(GameContract param)
        {
            try
            {
                IoC.Resolve<Lib.ICommand>("Endpoint", param).Execute();
                return "200";
            }
            catch (Exception e)
            {
                return IoC.Resolve<string>("Controller.Exception.Handle", e);
            }
        }
    }
}
