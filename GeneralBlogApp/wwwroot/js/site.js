const ul = document.querySelector(".tag-box ul");
const input = ul.querySelector("input");
const countNumb = document.querySelector(".details span");
let maxTags = 10,
tags = [];

//countTags();
//function countTags() {
//    countNumb.innerText = maxTags - tags.length;
//}
function createTag() {
    ul.querySelectorAll("li").forEach(li => li.remove());
    tags.slice().reverse().forEach(tag => {
        let liTag = `<li>${tag}<i class="fa-solid fa-xmark" onclick="removeTag(this, '${tag}')"></i></li>`;
        ul.insertAdjacentHTML("afterbegin", liTag);
    })
    /*countTags();*/
}

function removeTag(elem, tag) {
    let index = tags.indexOf(tag);
    tags = [...tags.slice(0, index), ...tags.slice(index + 1)];
    elem.parentElement.remove();
   /* countTags();*/
}
function addTag(e) {
    if (e.key == "Enter") {
        e.preventDefault();
        let tag = e.target.value.replace(/\s+/g, ' ');
        if (tag.length > 1 && !tags.includes(tag)) {
            tag.split(",").forEach(tag => {
                tags.push(tag);
                createTag();
            });
        }
        e.target.value = "";
    }

    
}
$(document).on("keydown", "form", function (event) {
    return event.key != "Enter";
});
input.addEventListener("keyup", addTag);
$('#createCatForm').submit(function (event) {
    // Add hidden input to store tags array in the form data
    $('<input>').attr({
        type: 'hidden',
        name: 'MetaKeywords',
        value: JSON.stringify(tags)
    }).appendTo('#createCatForm');

    // Uncomment the line below to prevent the form from actually submitting
    // event.preventDefault();
});