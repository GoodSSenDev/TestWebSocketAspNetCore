#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace NetworkModule
{
    /// <summary>
    /// Dispatches different type of messages into correct handler.
    /// </summary>
    /// <typeparam name="TSerializedDataType">Type of encoded message used. E.g. Json, XML</typeparam>
    public abstract class MessageDispatcher<TSerializedDataType> where TSerializedDataType : class, new()
    {
        //TODO: Change the dispatcher system to a better mechanism.
        //Stores message handlers 
        protected readonly List<(RouteAttribute route, Func<TSerializedDataType,Task<TSerializedDataType?>> targetMethod)> _handlers
            = new List<(RouteAttribute route, Func<TSerializedDataType, Task<TSerializedDataType?>> targetMethod)>();

        //private int _dispatchIndex = 0;

        //Bind dispatching action to a message process step of a channel when the channel get message.
        public void Bind( Channel<TSerializedDataType> channel )
            => channel.OnMessage( async m => {
                var response = await DispatchAsync(m).ConfigureAwait(false);
                if ( response != null ) {
                    try 
                    {
                        await channel.SendAsync( response ).ConfigureAwait( false );
                    } 
                    catch ( Exception ex ) 
                    {
                        Console.WriteLine( $"Error Occur channel OnMessage:: {ex}" );
                    }
                }
            } );

        //find a fit message handler for message received
        protected virtual async Task<TSerializedDataType?> DispatchAsync( TSerializedDataType message )
        {
            foreach ( var (route, target) in _handlers ) 
            {
                if ( IsMatch( route, message ) ) 
                {
                    return await target( message );
                }
            }

            return null;
        }

        //Add a message handler
        protected void AddHandler( RouteAttribute route, Func<TSerializedDataType, Task<TSerializedDataType?>> targetMethod )
            => _handlers.Add( (route, targetMethod) );

        //Check the message received is matched with message handler
        protected abstract bool IsMatch( RouteAttribute route, TSerializedDataType message );

        //Registers a message handler
        public virtual void Register<TMessage, TResult>(Func<TMessage, Task<TResult>> target)
        {
            if (!HasAttribute(target.Method))
                throw new Exception("Missing Required Route Attribute");
            //if (typeof(TMessage).IsSubclassOf(typeof(Message)))
            //{
            //    typeof(TMessage).GetProperty("DispatchIndex").SetValue(null, _dispatchIndex, null);

            //    _dispatchIndex++;
            //}
            var wrapper = new Func<TSerializedDataType, Task<TSerializedDataType?>>(async serializedData => {
                var @param = Deserialize<TMessage>(serializedData);
                var result = await target(@param);

                if (result != null)
                    return Serialize<TResult>(result);
                else
                    return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler(GetRouteAttribute(target.Method), wrapper);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        //Registers a message handler (method that does not return anything)
        public virtual void Register<TMessage>( Func<TMessage, Task> target )
        {

            if ( !HasAttribute( target.Method ) )
                throw new Exception( "Missing Required Route Attribute" );

            //if (typeof(TMessage).IsSubclassOf(typeof(Message)))
            //{
            //    typeof(TMessage).GetProperty("DispatchIndex").SetValue(null, _dispatchIndex, null);

            //    _dispatchIndex++;
            //}

            var wrapper = new Func<TSerializedDataType,Task<TSerializedDataType?>>( async serializedData=> {
                var @param = Deserialize<TMessage>(serializedData);
                await target(@param);
                return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
        }

        protected bool HasAttribute( MethodInfo mi ) => GetRouteAttribute( mi ) != null;
        protected abstract RouteAttribute? GetRouteAttribute( MethodInfo mi );

        protected abstract TParam Deserialize<TParam>(TSerializedDataType message);
        protected abstract object Deserialize(Type paramType, TSerializedDataType message);

        protected abstract TSerializedDataType? Serialize<T>(T instance);
    }
}
