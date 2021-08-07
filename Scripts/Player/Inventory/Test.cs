using System.Collections;
using System.Collections.Generic;

// TODO: delete this file
interface A  {

}

interface B : A {

}

class Test {
    public Test() {
        List<IList> listOfLists = new List<IList>();
        List<B> list = new List<B>();
        listOfLists.Add(list);
    }
}
