package yitek.workflow.core.std;

import java.io.*;
import java.nio.file.*;
import java.util.regex.Pattern;

import com.alibaba.fastjson.*;

import yitek.workflow.core.StringMap;


public class FileDiagramRepository implements DiagramRepository {
	String baseDir;
	public FileDiagramRepository(){
		this(null);
	}
	public FileDiagramRepository(String basDir){
		if(basDir==null || "".equals(basDir)){
			basDir = System.getProperty("user.dir") + "/wf-defs/";
		}
		this.baseDir = basDir;
	}
	public Diagram getDiagramByName(String name) 
	throws FileNotFoundException,UnsupportedEncodingException,IOException,SecurityException
	{
		 return this.getDiagramByName(name,null);
	}
	public Diagram getDiagramByName(String name,String version) 
	throws FileNotFoundException,UnsupportedEncodingException,IOException,SecurityException
	{
		String defFile = Paths.get(baseDir, name).toString();
		if(version!=null && !version.equals("")){
			defFile += "-v" + version;
		}
		defFile += ".json";
		BufferedReader reader = null;
        String readJson = "";
        try {
            FileInputStream fileInputStream = new FileInputStream(defFile);
            InputStreamReader inputStreamReader = new InputStreamReader(fileInputStream, "UTF-8");
            reader = new BufferedReader(inputStreamReader);
            String tempString = null;
            while ((tempString = reader.readLine()) != null){
				if(Pattern.matches("^\\s*//", tempString))continue;
                readJson += tempString + "\n";
            }
        } finally {
            if (reader != null){
                reader.close();
            }
        }
		System.out.println(readJson);
		return new Diagram(new StringMap(readJson),null);
	}	
}
