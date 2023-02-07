namespace EDTLibrary.UndoSystem;

public class UndoCommand
{
    public object Item { get; set; }
    public string PropName { get; set; }
    public object OldValue { get; set; }
    public object NewValue { get; set; }

    public override string ToString()
    {
        var output = $"Item: {Item}, Prop Name: {PropName}, New Value: {NewValue}, Old Value: {OldValue.ToString()}";
        return output;
    }
}
