namespace MVCAPP.Models;

enum Priority
{
    Low,
    Medium,
    High,
    Very_High,
    VH_Time = 10,
    H_Time = 24,
    M_Time =  72,
    L_Time = 168
}

//you can use the values but if you are assigning the values you need to convert it to int with 
//(int)Prioity.(level) this goes for checking to since its typye explict