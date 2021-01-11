package WF.graphs;

import java.util.*;
import lombok.*;
@Data
public class Element{
    String name;
    Map<String,String> variables;
    List<String> inParameters;
    List<String> outParameters;
}