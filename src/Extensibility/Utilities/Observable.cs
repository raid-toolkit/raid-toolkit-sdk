using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Raid.Toolkit.Extensibility.Utilities;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class DependsOnAttribute : Attribute
{
    public string PropertyName { get; }
    public DependsOnAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }
}

public class Observable : INotifyPropertyChanged
{
    private readonly Dictionary<string, object?> _data = new();
    private readonly Dictionary<string, PropertyChangedEventHandler> _listeners = new();
    private readonly Dictionary<string, string[]> _dependencies;

    public Observable()
    {
        PropertyInfo[] props = GetType().GetProperties();
        List<Tuple<string, string>> entries = new();
        foreach (PropertyInfo prop in props)
        {
            DependsOnAttribute[] attribs = prop.GetCustomAttributes<DependsOnAttribute>(true).ToArray();
            foreach (DependsOnAttribute attrib in attribs)
                entries.Add(new(attrib.PropertyName, prop.Name));
        }
        _dependencies = entries.GroupBy(rel => rel.Item1, rel => rel.Item2).ToDictionary(group => group.Key, group => group.ToArray());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null)
            return;

        HashSet<string> propertiesChanged = new() { propertyName };
        Queue<string> pendingProperties = new(new[] { propertyName });
        while (pendingProperties.TryDequeue(out string? prop))
        {
            if (!_dependencies.TryGetValue(prop, out string[]? dependents))
                continue;
            foreach (string dependent in dependents)
                if (propertiesChanged.Add(dependent))
                    pendingProperties.Enqueue(dependent); // only enqueue the first time
        }
        foreach (string changedPropertyName in propertiesChanged)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(changedPropertyName));
    }

    protected virtual bool SetField<T>(T value, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null) throw new NullReferenceException();
        T? currentValue = default;
        if (_data.TryGetValue(propertyName, out object? v))
        {
            currentValue = (T?)v;
        }

        if (EqualityComparer<T>.Default.Equals(currentValue, value)) return false;
        if (_listeners.TryGetValue(propertyName, out PropertyChangedEventHandler? oldListener))
            if (currentValue is INotifyPropertyChanged oldPropChanged)
                oldPropChanged.PropertyChanged -= oldListener;

        _data[propertyName] = value;
        if (value is INotifyPropertyChanged propChanged)
        {
            void handler(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(propertyName);
            _listeners[propertyName] = handler;
            propChanged.PropertyChanged += handler;
        }
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual T? GetField<T>([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null) throw new NullReferenceException();
        if (_data.TryGetValue(propertyName, out object? value))
            return (T?)value;
        return default;
    }

    protected virtual T GetField<T>(Func<T> defaultInitializer, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null) throw new NullReferenceException();
        if (_data.TryGetValue(propertyName, out object? value))
            return (T)value!;
        T initialValue = defaultInitializer();
        SetField(initialValue, propertyName);
        return initialValue;
    }
}
