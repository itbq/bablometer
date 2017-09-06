$(document).ready(function(){

    $(".loginPop").validate({
       rules:{
        
            login:{
                required: true,
                minlength: 4,
                maxlength: 16
            },
            
            pswd:{
                required: true,
                minlength: 1,
                maxlength: 16
            }
       },
       
       messages:{

            login:{
                required: true,
                messages: ""
            },

            pswd:{
                required: true,
                messages: ""
            }
        
       }
    });

}); //end of ready