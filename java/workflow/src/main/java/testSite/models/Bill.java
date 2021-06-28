package testSite.models;

import lombok.Data;

public interface Bill<T extends  Bill> {
    int getId();
    String getStatus();
    void setStatus(String t);
}
