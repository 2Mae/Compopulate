# Compopulate
**This is a work in progress. Use at your own risk.**

Associate `[Compopulate.Att]` with serialized component fields that are only meant to reference a component on the same gameObject. Run Compopulate to automatically fill them with `gameObject.GetComponent` and monitor discrepancies like missing or unexpected references.

It works as if you dragged and dropped in the inspector, but a lot faster and keyboard accessible. You can also remove or add `[Compopulate.Att]` to fields without breaking existing references.

Why not use GetComponent on Awake?
- Those references only exist at runtime.
- Initialisation order can cause trouble.
- Extra boilerplate and clutter in every script.
- Performance.

## Instructions
- Install via `Window/Package Manager/Add package from git URL...` and add `https://github.com/TTeig/Compopulate.git`
- Launch via `Window/Compopulate` or the listed keyboard shortcut.
- Use `[Compopulate.Att]` like below and follow the in-app instructions to process them in the editor.

```csharp
//Example usage:
[Compopulate.Att] public BoxCollider box;
[Compopulate.Att] public CustomComponent thing;
[Compopulate.Att] [SerializeField] private Rigidbody bod;
```

_I often use dictation and voice commands to write code and develop in Unity, but simulating mouse input to drag and drop is really cumbersome. So my main reason for making this tool is to make the editor more accessible and efficient for myself. But if someone else finds it useful or interesting, it would make me very happy._