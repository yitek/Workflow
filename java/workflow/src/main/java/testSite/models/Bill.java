package testSite.models;

import lombok.Data;

public interface Bill<T extends  Bill> {
    Integer getId();
    String getStatus();
    void setStatus(String t);
}
