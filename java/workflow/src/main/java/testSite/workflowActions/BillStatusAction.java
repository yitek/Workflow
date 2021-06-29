package testSite.workflowActions;

import org.springframework.stereotype.Service;
import testSite.BaseRepository;
import testSite.models.Bill;
import yitek.workflow.core.*;
import yitek.workflow.core.std.Transition;

@Service("billStatusAction")
public class BillStatusAction extends TaskAction {
    BaseRepository<Bill> _repository;

    @Override
	public Boolean entry(Activity activity ,Dealer dealer,StringMap inputs,DiagramBuilder builder,FlowContext ctx) throws Exception{
        
        return super.entry(activity, dealer, inputs, builder, ctx);
    }
    @Override
    public Object transfer(Activity activity,Dealer dealer,Transition transition,FlowContext ctx)throws Exception{
        String nextBillStatus = transition.getString("billStatus");
        String billService = activity.variables().getString("billService");
        if(billService==null) throw new Exception("节点"+activity.pathname() + "未配置billService");
        this._repository = (BaseRepository<Bill>) ctx.resolveInstance(billService);
        Bill bill = this._repository.selectByPrimaryKey(activity.billId());
        bill.setStatus(nextBillStatus);
        this._repository.updateByPrimaryKey(bill);
        return new StringMap();
    }
    
}
