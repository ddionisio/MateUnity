
public struct EntityEvent {
    public const string TriggerAct = "EntityTriggerAct";

    public const string ActionEnter = "EntityActionEnter";
    public const string ActionFinish = "EntityActionFinish";
    public const string ActionHitEnter = "EntityActionHitEnter";
    public const string ActionHitExit = "EntityActionHitExit";
    public const string Kill = "EntityKill";
    public const string Release = "EntityRelease";
    public const string Remove = "EntityRemove";
    public const string Resume = "EntityResume";
    public const string Sleep = "EntitySleep";
    public const string Start = "EntityStart";
    public const string Spawn = "EntitySpawn";
    public const string Wake = "EntityWake";
    public const string StatChanged = "EntityStatChanged";
    public const string TriggerEnter = "EntityActionTriggerEnter";
    public const string TriggerExit = "EntityActionTriggerExit";
    public const string StateChanged = "EntityStateChanged";
    public const string Restore = "EntityRestore"; //for some cases when we want entity to return to a state (undo, reload, etc.)
    public const string Save = "EntitySave"; //if there is a need to save some states for later restore
}
