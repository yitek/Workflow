package yitek.workflow.core;

import java.util.*;

import com.alibaba.fastjson.*;

public class StringMap implements Map<String,Object> {
	Map<String,Object> _map;
	private Boolean _readonly;
	static StringMap _empty = new StringMap().readonly(true);
	public static StringMap empty(){
		return _empty;
	}
	public StringMap(){
		this._map = new HashMap<String,Object>();
		this._readonly = false;
	}
	public StringMap(Map<String,Object> data){
		this._map = data;
		this._readonly = false;
	}
	@SuppressWarnings("unchecked")
	public StringMap(Object obj){
		this._readonly = false;
		if(obj instanceof Map) this._map = (Map<String,Object>)obj;
		else {
			Object json = JSON.toJSON(obj);
			if(json instanceof JSONObject) this._map = (Map<String,Object>)json;
			else
				this._map = new HashMap<>();
		}
	}

	public Set<Map.Entry<String,Object>> entrySet(){
		return this._map.entrySet();
	}
	public Set<String> keySet(){
		return this._map.keySet();
	}
	public Collection<Object> values(){
		return this._map.values();
	}
	public boolean isEmpty(){
		return this._map.isEmpty();
	}
	public int size(){
		return this._map.size();
	}
	public boolean containsKey(Object key){
		if(key==null){
			for(Object k:this._map.keySet()){
				if(k==null) return true;
			}
			return false;
		}else {
			for(Object k:this._map.keySet()){
				if(k==null) continue;
				if(k.equals(key)) return true;
			}
			return false;
		}
	}
	public boolean containsValue(Object value){
		return this._map.containsValue(value);
	}

	public Object remove(Object key){
		return this._map.remove(key);
	}
	public void putAll(Map<? extends String, ? extends Object> m){
		this._map.putAll(m);
	}

	public void clear(){
		this._map.clear();
	}
	
	public Object put(String key,Object value){
		if(this._readonly) return null;
		return this._map.put(key,value);
	}
	@Override

	public Object get(Object key){
		return resolve((Object)this._map,key==null?null:key.toString());
	}
	public Object get(String key){
		if(key==null){
			for(Map.Entry<String,Object> entry : this.entrySet()){
				if(entry.getKey()==null) return entry.getValue();
			}
			return null;
		}else{
			if(key.indexOf(".")>0){
				String[] names = key.split(".");
				return (Object)resolve((Object)this._map,names);
			}
			for(Map.Entry<String,Object> entry : this.entrySet()){
				if(entry.getKey()!=null && key.equals(entry.getKey())) return entry.getValue();
			}
			return null;
		}
	}

	public Boolean readonly(){
		return this._readonly;
	}
	public StringMap readonly(Boolean value){
		this._readonly = value;
		return this;
	}
	public StringMap clone(){
		return (StringMap)cloneJSON(this);
	}
	@SuppressWarnings("unchecked")
	public static Object cloneJSON(Object obj){
		if(obj instanceof JSONArray){
			List<Object> rs = new ArrayList<Object>();
			for(Object item : (JSONArray)obj){
				rs.add(cloneJSON(item));
			}
			return rs;
		}else if(obj instanceof JSONObject){
			StringMap rs = new StringMap();
			for(Map.Entry<String,Object> pair : ((JSONObject)obj).entrySet()){
				rs.put(pair.getKey(), cloneJSON(pair.getValue()));
			}
			return rs;
		}else if(obj instanceof Map){
			StringMap rs = new StringMap();
			for(Map.Entry<String,Object> pair : ((Map<String,Object>)obj).entrySet()){
				rs.put(pair.getKey(), cloneJSON(pair.getValue()));
			}
			return rs;
		}else if(obj instanceof List){
			List<Object> rs = new ArrayList<Object>();
			for(Object item : ((List<Object>)obj)){
				rs.add(cloneJSON(item));
			}
			return rs;
		}
		else return obj;
	}

	
	@SuppressWarnings("unchecked")
	public static Object resolve(Object map,String name){
		if(map instanceof Map){
			Map<String,Object> mapObj = (Map<String,Object>)map;
			return resolve(mapObj,name);
		}
		Object json = JSON.toJSON(map);
		if(json instanceof JSONObject){
			return resolve(json,name);
		}else return null;
		
	}
	public static <T> T resolve(Map<String,T> map,String name){
		if(map==null) return null;
		if(name==null){
			for(Map.Entry<String,T> entry : map.entrySet()){
				if(entry.getKey()==null) return entry.getValue();
			}
			return null;
		}else {
			for(Map.Entry<String,T> entry : map.entrySet()){
				Object key = entry.getKey();
				if(key==null) continue;
				if(key.toString().equals(name)) return entry.getValue();
			}
			return null;
		}
		
	}
	
	public static Object resolve(Object map,String[] pathnames,int start){
		for(int i=start;i<pathnames.length;i++){
			String pathname = pathnames[i];
			map = resolve(map, pathname);
			if(map==null) return null;
		}
		return map;
	}
	public static Object resolve(Object map,String[] pathnames){
		return resolve(map, pathnames,0);
	}
}
