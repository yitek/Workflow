package yitek.workflow.core.std;

import java.io.*;

public interface DiagramRepository {
	Diagram getDiagramByName(String name,String version) throws FileNotFoundException,UnsupportedEncodingException,IOException,SecurityException;
	Diagram getDiagramByName(String name) throws FileNotFoundException,UnsupportedEncodingException,IOException,SecurityException;
}
