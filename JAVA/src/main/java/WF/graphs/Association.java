package WF.graphs;
import lombok.*;
@Data
@EqualsAndHashCode(callSuper=false)
public class Association extends Element{
    String from;
    String to;
}

