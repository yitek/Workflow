package WF.activities;

import WF.*;
import WF.graphs.*;
import java.util.*;
import java.util.stream.*;

public class Activity {
    UUID id;
    UUID flowId;
    Node defNode;
    Activity parent;
    Map<String,String> states;
    ActivityStates status;
    Activity from;
    Activity prev;
    Association association;

    public Activity(){
        this.states = new HashMap<String,String>();
    }
    public String state(String name){
        return this.states.get(name);
    }

    public Activity execute(IUser user,Map<String,String> inputs) throws Exception{
        boolean stateChanges = false;
        boolean statusChanged = false;
        // 初始化阶段
        if(this.status==null){
            try{
                // 导入父级的states到当前的activity状态中
                if(this.initialize(inputs)) stateChanges = true;
                this.status = ActivityStates.Initialized;
                statusChanged = true;
            }catch(Exception ex){
                this.status = null;
                throw ex;
            }
            
        }
        // 执行阶段
        if(ActivityStates.Initialized.equals(this.status)){
            try{
                // 执行处理，如果处理有返回，说明
                Boolean hasChanges = this.dealing(user, inputs);
                if(hasChanges!=null){
                    if(hasChanges) stateChanges = true;
                    this.status = ActivityStates.Dealed;
                    statusChanged = true;
                }
            }catch(Exception ex){
                this.status = ActivityStates.Initialized;
                throw ex;
            }
        }
        List<Activity> nextActivities = null;
        if(ActivityStates.Dealed.equals(this.status)){
            nextActivities = this.resolveNextActivities(this.states);
            if(nextActivities!=null){
                try{
                    this.insertNewActivities(nextActivities);
                    this.status = ActivityStates.Done;
                    statusChanged = true;
                }catch(Exception ex){
                    this.status = ActivityStates.Dealed;
                    throw ex;
                }
            }
        }
        this.storeStates(stateChanges, statusChanged);      
        return this;
    }
    
    boolean initialize(Map<String,String> inputs){
        Boolean hasChanges = false;
        // 将父级的状态引入当前Activity
        Map<String,String> inParams = this.defNode.getInParameters();
        if(this.parent!=null && inParams!=null) {
            for(String pname : inParams.keySet()){
                this.states.put(pname,this.parent.state(inParams.get(pname)));
                hasChanges = true;
            }
        }
        if(inputs!=null){
            for(String key:inputs.keySet()){
                this.states.put(key,inputs.get(key));
                hasChanges = true;
            }
        }
        return hasChanges;
    }
    Boolean dealing(IUser user, Map<String,String> inputs){
        Map<String,String> result = this.deal(user, inputs);
        if(result==null) return null;
        Boolean hasChanges = false;
        for(String key : result.keySet()){
            this.states.put(key,result.get(key));
            hasChanges = true;
        }
        exportStates();
        return hasChanges;

    }

    void exportStates(){
        Map<String,String> outParams = this.defNode.getOutParameters();
        if(outParams!=null){
            for(String statename : outParams.keySet()){
                this.parent.states.put(outParams.get(statename),this.states.get(statename));
            }
            // export 变更了父级acitity的状态，保存
            this.parent.storeStates(true, false);
        }
    }

    

    protected Map<String,String> deal(IUser user,Map<String,String> inputs){
        return null;
    }

    

    List<Activity> resolveNextActivities(Map<String,String> map) throws Exception {
        if(this.parent==null) return new ArrayList<Activity>();
        //找到父级节点的定义
        
        //从父级节点的定义中获取到所有的连线
        List<Association> assocs = this.parent.defNode.getAssociations();
        //如果没有连线，说明只有一个子节点，大流程结束
        if(assocs==null) return new ArrayList<Activity>();
        // 找到以当前节点为起点的连线
        List<Association> nexts = assocs.stream().filter(p->p.getFrom() == this.defNode.getName()).collect(Collectors.toList());
        // 没有，说明是结束节点，返回end
        if(nexts==null || nexts.size()==0)  return new ArrayList<Activity>();

        Boolean statesExported = false;
        
        List<Activity> nextActivities =  new ArrayList<Activity>();
        // 循环找到连线
        for(Association assoc : assocs){
            // 找到连线链接的节点定义
            Node nextNode = resolveNextNode(assoc,this.states);
            if (nextNode==null) { continue;}
            if(!statesExported) {
                
                statesExported = true;
            }
            Activity nextActivity = createNextActivity(nextNode,assoc);
            nextActivities.add(nextActivity);
            
        }
        return nextActivities.size()==0?null:nextActivities;
    }

    protected Node resolveNextNode(Association assoc,Map<String,String> states) throws Exception {
        Node pNode = this.parent.defNode;
        Node nextNode = pNode.getNodes().stream().filter(p->p.getName()==assoc.getTo()).findFirst().orElse(null);
        // 线条的另外一头没有节点，错误的图形~~
        if(nextNode==null) throw new Exception("未找到" + assoc.getTo() + "节点");
        String key = assoc.getKey();
        if(key==null) return nextNode;
        String value = assoc.getValue();

        String stateValue = states.get(key);
        if(value==null) {
            if(stateValue==null) return nextNode;
            return null;
        }else {
            if (value.equals(stateValue)) return nextNode;
            return null;
        }
    }

    public Activity createNextActivity(Node node,Association assoc) throws ClassNotFoundException{
        String typename = node.getTypename();
        Activity activity = null;
        if(typename==null || "".equals(typename)){
            activity = new Activity();
        }else {
            activity = (Activity)createInstance(typename);
        }
        activity.defNode = node;
        activity.parent = this.parent;
        activity.prev = this;
        activity.association =assoc;
        return activity;
    }

    void insertNewActivities(List<Activity> activities){

    }

    public static Object createInstance(String className)
    throws ClassNotFoundException{
             
        try {
            Class<?> clz = Class.forName(className);
            Object obj = clz.getDeclaredConstructor().newInstance() ;
            
            return obj;
        } catch (Exception e) {
            e.printStackTrace();
        }
         
        return null;
    }

    protected void storeStates(boolean statesChanged,boolean statusChanged){

    }

}
