package testSite.workflowActions;

import org.springframework.stereotype.Service;
import testSite.BaseRepository;
import testSite.models.Bill;
import yitek.workflow.core.*;

import java.util.HashMap;

@Service
public class BillStatusAction extends TaskAction {
    BaseRepository<Bill> _repository;

    @Override
    public Object deal(Activity activity, Dealer dealer, StringMap params, FlowContext ctx) {

        Object action = params.get("action");
        if(action==null) return null;
        super.deal(activity,dealer,params,ctx);
        String billService = activity.variables("billService").toString();
        String billStatus = activity.variables("billStatus").toString();
        this._repository = (BaseRepository<Bill>) ctx.resolveInstance(billService);
        Bill bill = this._repository.selectByPrimaryKey(activity.billId());
        bill.setStatus(billStatus);
        this._repository.updateByPrimaryKey(bill);
        return new HashMap<String,Object>(){
            {
                put("billStatus", billStatus);
                put("dealer",dealer);
            }
        };
    }
}
