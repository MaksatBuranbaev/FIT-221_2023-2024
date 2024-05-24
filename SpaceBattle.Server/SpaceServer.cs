using System.Diagnostics.CodeAnalysis;
using CoreWCF;
using Hwdtech;
using SpaceBattle.Lib;
using System;

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
                IoC.Resolve<IExceptionHandler>("Controller.Exception.Handle", e).Handle();
                return "400";
            }
        }
    }
}
