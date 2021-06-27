package yitek.workflow.core;

import com.alibaba.fastjson.*;

import yitek.workflow.core.std.*;

public interface Action {
	Diagram entry(Dealer dealer,Object inputs,JSONObject variables,Diagram subDiagram,BusinessInfo businessInfo,TriggleContext ctt);	
	Object deal(Dealer dealer,Object params,TriggleContext ctx);
	Boolean exit(Dealer dealer,TriggleContext ctx);
}
