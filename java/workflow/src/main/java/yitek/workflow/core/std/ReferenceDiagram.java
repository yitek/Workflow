package yitek.workflow.core.std;

public class ReferenceDiagram extends Diagram{
    public ReferenceDiagram(String name){
        this._referenceName = name;
    }
    public String _referenceName;
    public String reference(){
        return this._referenceName;
    }

}
