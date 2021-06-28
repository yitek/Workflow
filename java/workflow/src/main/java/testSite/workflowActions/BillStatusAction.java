package testSite.workflowActions;

import testSite.BaseRepository;
import testSite.models.Bill;
import yitek.workflow.core.*;

import java.util.HashMap;

public class BillAction<TBill extends Bill> extends TaskAction {
    BaseRepository<TBill> _repository;

    @Override
    public Object deal(Activity activity, Dealer dealer, StringMap params, FlowContext ctx) {
        super.deal(activity,dealer,params,ctx);
        String billService = activity.variables("billService").toString();
        String billStatus = activity.variables("billStatus").toString();
        this._repository = (BaseRepository<TBill>) ctx.resolveInstance(billService);
        TBill bill = this._repository.selectByPrimaryKey(activity.billId());
        bill.setStatus(billStatus);
        this._repository.updateByPrimaryKey(bill);
        return new HashMap<String,String>(){
            {
                put("billStatus", billStatus);
            }
        };
    }
}
