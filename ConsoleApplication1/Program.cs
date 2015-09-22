using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main( string[] args )
        {
            ChangePWDCommand cd = new ChangePWDCommand()
            {
                OldCommand = "a",
                NewCommand = "b"
            };
            ( new CommandBus() ).Execute( cd );

            Console.Read();
        }
    }

    public interface ICommand
    {

    }

    public interface ICommandHandler<TCommand> where TCommand : class,ICommand
    {
        void Handle( TCommand command );
    }

    public class CommandBus
    {
        public void Execute<T>(T cmd) where T:class,ICommand
        {
            /*
             * 首先查找当前的程序集中(ICommandHandler)所在的程序集中的所有的实现了ICommandHandler的接口的类型，然后在所有的类型找查找实现了该泛型接口并且泛型的类型参数类型为T类型的所有类型。
             */

            var handlers = typeof( ICommandHandler<> ).Assembly.GetExportedTypes()
                .Where
                ( 
                    t => t.GetInterfaces()
                        .Any
                        (
                            x=>x.IsGenericType
                            &&
                            x.GetGenericTypeDefinition()==typeof(ICommandHandler<>)
                        )
                )
                .Where
                ( 
                    h => h.GetInterfaces()
                        .Any
                        ( 
                            j => j.GetGenericArguments()
                                .Any( arg => arg == typeof( T ) ) 
                        ) 
                );
            if ( handlers.Count() >= 0 )
            {
                var handler = handlers.FirstOrDefault() ;
                ( Activator.CreateInstance( handler ) as ICommandHandler<T> ).Handle( cmd );
            }
        }
    }

    public class ChangePWDCommand:ICommand
    {
        public string OldCommand { get; set; }
        public string NewCommand{get;set;}
    }
        
    public class ChangePWDCommandHandler : ICommandHandler<ChangePWDCommand>
    {
        public void Handle( ChangePWDCommand cmd ) 
        {
            Console.WriteLine( "oldpwd:{0},newpwd:{1}", cmd.OldCommand, cmd.NewCommand );
        }
    }
}
