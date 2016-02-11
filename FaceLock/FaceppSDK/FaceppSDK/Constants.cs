using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceppSDK
{
    class Constants
    {
        public static String FACE_KEY = "2affcadaeddd18f422375adc869f3991";
        public static String FACE_SECRET = "EsU9hmgweuz8U-nwv6s4JP-9AJt64vhz";
	
	    public static  String URL_DETECT = "https://apius.faceplusplus.com/detection/detect"; 
	    public static  String URL_COMPARE = "https://apius.faceplusplus.com/recognition/compare"; 
	    public static  String URL_RECOGNIZE = "https://apius.faceplusplus.com/recognition/recognize"; 
	    public static  String URL_SEARCH = "https://apius.faceplusplus.com/recognition/search"; 
	    public static  String URL_TRAIN = "https://apius.faceplusplus.com/recognition/train"; 
	    public static  String URL_VERIFY = "https://apius.faceplusplus.com/recognition/verify"; 
	
	    public static  String URL_PERSON_ADDFACE = "https://apius.faceplusplus.com/person/add_face";
	    public static  String URL_PERSON_CREATE = "https://apius.faceplusplus.com/person/create";
	    public static  String URL_PERSON_DELETE = "https://apius.faceplusplus.com/person/delete";
	    public static  String URL_PERSON_GETINFO = "https://apius.faceplusplus.com/person/get_info";
	    public static  String URL_PERSON_REMOVEFACE = "https://apius.faceplusplus.com/person/remove_face";
	    public static  String URL_PERSON_SETINFO = "https://apius.faceplusplus.com/person/set_info";
	
	    public static  String URL_GROUP_ADDPERSON = "https://apius.faceplusplus.com/group/add_person";
	    public static  String URL_GROUP_CREATE = "https://apius.faceplusplus.com/group/create";
	    public static  String URL_GROUP_DELETE = "https://apius.faceplusplus.com/group/delete";
	    public static  String URL_GROUP_GETINFO = "https://apius.faceplusplus.com/group/get_info";
	    public static  String URL_GROUP_REMOVEPERSON = "https://apius.faceplusplus.com/group/remove_person";
	    public static  String URL_GROUP_SETINFO = "https://apius.faceplusplus.com/group/set_info";

        public static String URL_INFO_GETAPP = "https://apius.faceplusplus.com/info/get_app";
        public static String URL_INFO_GETFACE = "https://apius.faceplusplus.com/info/get_face";
        public static String URL_INFO_GETGROUPLIST = "https://apius.faceplusplus.com/info/get_group_list";
        public static String URL_INFO_GETIMAGE = "https://apius.faceplusplus.com/info/get_image";
        public static String URL_INFO_GETPERSONLIST = "https://apius.faceplusplus.com/info/get_person_list";
        public static String URL_INFO_GETQUOTA = "https://apius.faceplusplus.com/info/get_quota";
        public static String URL_INFO_GETSESSION = "https://apius.faceplusplus.com/info/get_session";
        public static String URL_INFO_GET_FACESETLIST = "https://apius.faceplusplus.com/info/get_faceset_list";

        public static String URL_FACESET_CREATE = "https://apius.faceplusplus.com/faceset/create";
        public static String URL_FACESET_DELETE = "https://apius.faceplusplus.com/faceset/delete";
        public static String URL_FACESET_ADDFACE = "https://apius.faceplusplus.com/faceset/add_face";
        public static String URL_FACESET_REMOVEFACE = "https://apius.faceplusplus.com/faceset/remove_face";
        public static String URL_FACESET_SETINFO = "https://apius.faceplusplus.com/faceset/set_info";
        public static String URL_FACESET_GET_INFO = "https://apius.faceplusplus.com/faceset/get_info";

        public static String URL_TRAIN_VERIFY = "https://apius.faceplusplus.com/train/verify";
        public static String URL_TRAIN_SEARCH = "https://apius.faceplusplus.com/train/search";
        public static String URL_TRAIN_IDENTIFY = "https://apius.faceplusplus.com/train/identify";

        public static String URL_GROUPING_GROUPING = "https://apius.faceplusplus.com/grouping/grouping";
    }
}
