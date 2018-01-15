# Scripting (OrchardCore.Scripting)

## Purpose

The scripting provides an API allowing you to evaluate custom scripts in different languages.

## Usage

### Executing some script

The main interface is `IScriptingManager`.
```
public interface IScriptingManager
{
    IScriptingEngine GetScriptingEngine(string prefix);
    object Evaluate(string directive);  
    IList<IGlobalMethodProvider> GlobalMethodProviders { get; }
}
```

To evaluate an expression using a scripting engine, you must know which ones are available in the system. 
For instance, a JavaScript one is available by default and its prefix is `js`.
To return the current date and time as a string we could to something like this:

```
var scriptingManager = serviceProvider.GetService<IScriptingManager>();
var date = scriptingManager.Evaluate("js: Date().toISOString()");
```

The `js:` prefix is used to describe in which language the code is written. Any module can provide
a new scripting engine by implementing the `IScriptingEngine` interface.

### Customizing the scripting environment

Any module can provide custom methods for scripts independently of the chosen language. 
For instance the Contents module provides a `uuid()` helper method that computes a unique content item identifier.

To create a global method implement `IGlobalMethodProvider` then add it to the current `IScriptingManager` 
instance like this:

```
var scriptingManager = serviceProvider.GetService<IScriptingManager>();
var globalMethodProvider = new MyGlobalMethodProvider();
scriptingManager.GlobalMethodProviders.Add(globalMethodProvider);
```
