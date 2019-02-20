// Updated Ultimate WPF Event Method Binding implementation by Mike Marynowski
// View the article here: http://www.singulink.com/CodeIndex/post/updated-ultimate-wpf-event-method-binding

// Licensed under the Code Project Open License: http://www.codeproject.com/info/cpol10.aspx

using NullGuard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Singulink.Windows.Data
{
    public static class PropertyPathHelpers
    {
        #region Public Methods

        public static object Evaluate(PropertyPath path, object source)
        {
            var target = new DependencyTarget();
            var binding = new Binding() { Path = path, Source = source, Mode = BindingMode.OneTime };
            BindingOperations.SetBinding(target, DependencyTarget.ValueProperty, binding);

            return target.Value;
        }

        #endregion Public Methods

        #region Private Classes

        private class DependencyTarget : DependencyObject
        {
            #region Public Fields

            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DependencyTarget));

            #endregion Public Fields

            #region Public Properties

            public object Value
            {
                get => GetValue(ValueProperty);
                set => SetValue(ValueProperty, value);
            }

            #endregion Public Properties
        }

        #endregion Private Classes
    }

    public class EventArgsExtension : MarkupExtension
    {
        #region Public Constructors

        public EventArgsExtension()
        {
        }

        public EventArgsExtension(string path)
        {
            Path = new PropertyPath(path);
        }

        #endregion Public Constructors

        #region Public Properties

        public IValueConverter Converter { get; set; }

        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        public object ConverterParameter { get; set; }

        public Type ConverterTargetType { get; set; }

        [AllowNull]
        public PropertyPath Path { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion Public Methods

        #region Internal Methods

        internal object GetArgumentValue(EventArgs eventArgs, XmlLanguage language)
        {
            if (Path == null)
                return eventArgs;

            object value = PropertyPathHelpers.Evaluate(Path, eventArgs);

            if (Converter != null)
                value = Converter.Convert(value, ConverterTargetType ?? typeof(object), ConverterParameter, ConverterCulture ?? language.GetSpecificCulture());

            return value;
        }

        #endregion Internal Methods
    }

    public class EventSenderExtension : MarkupExtension
    {
        #region Public Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion Public Methods
    }

    public class MethodBindingExtension : MarkupExtension
    {
        #region Private Fields

        private static readonly List<DependencyProperty> StorageProperties = new List<DependencyProperty>();

        private readonly List<DependencyProperty> _argumentProperties = new List<DependencyProperty>();

        private readonly object[] _arguments;

        #endregion Private Fields

        #region Public Constructors

        public MethodBindingExtension(object method) : this(new[] { method })
        {
        }

        public MethodBindingExtension(object arg0, object arg1) : this(new[] { arg0, arg1 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2) : this(new[] { arg0, arg1, arg2 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2, object arg3) : this(new[] { arg0, arg1, arg2, arg3 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2, object arg3, object arg4) : this(new[] { arg0, arg1, arg2, arg3, arg4 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 })
        {
        }

        public MethodBindingExtension(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 })
        {
        }

        #endregion Public Constructors

        #region Private Constructors

        private MethodBindingExtension(object[] arguments)
        {
            _arguments = arguments;
        }

        #endregion Private Constructors

        #region Public Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var target = provideValueTarget.TargetObject as FrameworkElement;
            Type eventHandlerType = null;

            if (provideValueTarget.TargetProperty is EventInfo eventInfo)
            {
                eventHandlerType = eventInfo.EventHandlerType;
            }
            else if (provideValueTarget.TargetProperty is MethodInfo methodInfo)
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length == 2)
                    eventHandlerType = parameters[1].ParameterType;
            }

            if (target == null || eventHandlerType == null)
                return this;

            foreach (object argument in _arguments)
            {
                var argumentProperty = SetUnusedStorageProperty(target, argument);
                _argumentProperties.Add(argumentProperty);
            }

            return CreateEventHandler(target, eventHandlerType);
        }

        #endregion Public Methods

        #region Private Methods

        private Delegate CreateEventHandler(FrameworkElement element, Type eventHandlerType)
        {
            EventHandler handler = (sender, eventArgs) =>
            {
                object arg0 = element.GetValue(_argumentProperties[0]);

                if (arg0 == null)
                {
                    Debug.WriteLine("[MethodBinding] First method binding argument is required and cannot resolve to null - method name or method target expected.");
                    return;
                }

                int methodArgsStart;
                object methodTarget;

                // If the first argument is a string then it must be the name of the method to invoke on the data context.
                // If not then it is the excplicit method target object and the second argument will be name of the method to invoke.

                if (arg0 is string methodName)
                {
                    methodTarget = element.DataContext;
                    methodArgsStart = 1;
                }
                else if (_argumentProperties.Count >= 2)
                {
                    methodTarget = arg0;
                    methodArgsStart = 2;

                    object arg1 = element.GetValue(_argumentProperties[1]);

                    if (arg1 == null)
                    {
                        Debug.WriteLine($"[MethodBinding] First argument resolved as a method target object of type '{methodTarget.GetType()}', second argument must resolve to a method name and cannot resolve to null.");
                        return;
                    }

                    methodName = arg1 as string;

                    if (methodName == null)
                    {
                        Debug.WriteLine($"[MethodBinding] First argument resolved as a method target object of type '{methodTarget.GetType()}', second argument (method name) must resolve to a '{typeof(string)}' (actual type: '{arg1.GetType()}').");
                        return;
                    }
                }
                else
                {
                    Debug.WriteLine($"[MethodBinding] Method name must resolve to a '{typeof(string)}' (actual type: '{arg0.GetType()}').");
                    return;
                }

                var arguments = new object[_argumentProperties.Count - methodArgsStart];

                for (int i = methodArgsStart; i < _argumentProperties.Count; i++)
                {
                    object argValue = element.GetValue(_argumentProperties[i]);

                    if (argValue is EventSenderExtension)
                        argValue = sender;
                    else if (argValue is EventArgsExtension eventArgsEx)
                        argValue = eventArgsEx.GetArgumentValue(eventArgs, element.Language);

                    arguments[i - methodArgsStart] = argValue;
                }

                var methodTargetType = methodTarget.GetType();

                // Try invoking the method by resolving it based on the arguments provided

                try
                {
                    methodTargetType.InvokeMember(methodName, BindingFlags.InvokeMethod, null, methodTarget, arguments);
                    return;
                }
                catch (MissingMethodException) { }

                // Couldn't match a method with the raw arguments, so check if we can find a method with the same name
                // and parameter count and try to convert any XAML string arguments to match the method parameter types

                var method = methodTargetType.GetMethods().SingleOrDefault(m => m.Name == methodName && m.GetParameters().Length == arguments.Length);

                if (method != null)
                {
                    var parameters = method.GetParameters();

                    for (int i = 0; i < _arguments.Length; i++)
                    {
                        if (arguments[i] == null)
                        {
                            if (parameters[i].ParameterType.IsValueType)
                            {
                                method = null;
                                break;
                            }
                        }
                        else if (_arguments[i] is string && parameters[i].ParameterType != typeof(string))
                        {
                            // The original value provided for this argument was a XAML string so try to convert it
                            arguments[i] = TypeDescriptor.GetConverter(parameters[i].ParameterType).ConvertFromString((string)_arguments[i]);
                        }
                        else if (!parameters[i].ParameterType.IsInstanceOfType(arguments[i]))
                        {
                            method = null;
                            break;
                        }
                    }

                    method?.Invoke(methodTarget, arguments);
                }

                if (method == null)
                    Debug.WriteLine($"[MethodBinding] Could not find a method '{methodName}' on target type '{methodTargetType}' that accepts the parameters provided.");
            };

            return Delegate.CreateDelegate(eventHandlerType, handler.Target, handler.Method);
        }

        private DependencyProperty SetUnusedStorageProperty(DependencyObject obj, object value)
        {
            var property = StorageProperties.FirstOrDefault(p => obj.ReadLocalValue(p) == DependencyProperty.UnsetValue);

            if (property == null)
            {
                property = DependencyProperty.RegisterAttached("Storage" + StorageProperties.Count, typeof(object), typeof(MethodBindingExtension), new PropertyMetadata());
                StorageProperties.Add(property);
            }

            var markupExtension = value as MarkupExtension;

            if (markupExtension != null)
            {
                var resolvedValue = markupExtension.ProvideValue(new ServiceProvider(obj, property));
                obj.SetValue(property, resolvedValue);
            }
            else
            {
                obj.SetValue(property, value);
            }

            return property;
        }

        #endregion Private Methods

        #region Private Classes

        private class ServiceProvider : IServiceProvider, IProvideValueTarget
        {
            #region Public Constructors

            public ServiceProvider(object targetObject, object targetProperty)
            {
                TargetObject = targetObject;
                TargetProperty = targetProperty;
            }

            #endregion Public Constructors

            #region Public Properties

            public object TargetObject { get; }

            public object TargetProperty { get; }

            #endregion Public Properties

            #region Public Methods

            public object GetService(Type serviceType)
            {
                return serviceType.IsInstanceOfType(this) ? this : null;
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}