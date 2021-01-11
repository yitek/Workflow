package WF;

import WF.graphs.*;
import java.io.*;
import java.nio.file.Path;

import com.alibaba.fastjson.*;

class Engine{
    public static Node loadGraphFromJsonFile(String file){
        String jsonText = readFileContent(file);
        return JSON.parseObject(jsonText, Node.class);
    }
    public static Node loadGraphFromName(String fullname){
        String baseDir = System.getProperty("user.dir");
        String filename = baseDir + "/wf-defs/" + fullname + ".json";
        System.out.println(filename);
        return loadGraphFromJsonFile(filename);
    }
    public static String readFileContent(String fileName) {
        File file = new File(fileName);
        BufferedReader reader = null;
        StringBuffer sbf = new StringBuffer();
        try {
            reader = new BufferedReader(new FileReader(file));
            String tempStr;
            while ((tempStr = reader.readLine()) != null) {
                sbf.append(tempStr);
            }
            reader.close();
            return sbf.toString();
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            if (reader != null) {
                try {
                    reader.close();
                } catch (IOException e1) {
                    e1.printStackTrace();
                }
            }
        }
        return sbf.toString();
    }
}