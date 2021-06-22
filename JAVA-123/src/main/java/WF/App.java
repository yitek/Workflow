package WF;

import WF.graphs.Node;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        Node node = Engine.loadGraphFromName("sample.wf");
        System.out.println( "Hello World!" );
    }
}
