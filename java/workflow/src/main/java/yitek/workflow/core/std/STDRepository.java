package yitek.workflow.core.std;

import java.io.*;

public interface STDRepository {
	State GetStateByName(String name,String version) throws FileNotFoundException,UnsupportedEncodingException,IOException,SecurityException;
	State GetStateByName(String name) throws FileNotFoundException,UnsupportedEncodingException,IOException,SecurityException;
}
