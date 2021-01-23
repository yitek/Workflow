package WF.graphs;

import lombok.*;
import java.util.*;

@Data
@EqualsAndHashCode(callSuper=false)
public class Node extends Element{
    Map<String,String> inParameters;
    Map<String,String> outParameters;

    NodeTypes type;
    List<Node> nodes;
    List<Association> associations;
    String startNode;

}