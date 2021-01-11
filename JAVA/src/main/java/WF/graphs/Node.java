package WF.graphs;

import lombok.*;
import java.util.*;

@Data
@EqualsAndHashCode(callSuper=false)
public class Node extends Element{
    NodeTypes type;
    List<Node> nodes;
    List<Association> assications;
    String startNode;

}