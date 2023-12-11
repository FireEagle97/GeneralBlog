class TagManager {
    constructor(ulSelector, inputSelector, countNumbSelector, maxTags) {
        this.ul = document.querySelector(ulSelector);
        this.input = this.ul.querySelector(inputSelector);
/*        this.countNumb = document.querySelector(countNumbSelector);*/
        this.maxTags = maxTags || 10;
        this.tags = [];

        // Initialize the tag manager
        this.init();
    }

    init() {
        this.input.addEventListener("keyup", this.addTag.bind(this));
        // Add any additional initialization logic here
    }

    createTag() {
        this.ul.querySelectorAll("li").forEach(li => li.remove());
        this.tags.slice().reverse().forEach(tag => {
            let liTag = `<li>${tag}<i class="fa-solid fa-xmark" onclick="removeTag(this, '${tag}')"></i></li>`;
            this.ul.insertAdjacentHTML("afterbegin", liTag);
        });
        /*this.countTags();*/
    }

    removeTag(elem, tag) {
        let index = this.tags.indexOf(tag);
        this.tags = [...this.tags.slice(0, index), ...this.tags.slice(index + 1)];
        elem.parentElement.remove();
       /* this.countTags();*/
    }

    addTag(e) {
        if (e.key == "Enter") {
            e.preventDefault();
            let tag = e.target.value.replace(/\s+/g, ' ');
            if (tag.length > 1 && !this.tags.includes(tag)) {
                tag.split(",").forEach(tag => {
                    this.tags.push(tag);
                    this.createTag();
                });
            }
            e.target.value = "";
        }
    }

    //countTags() {
    //    this.countNumb.innerText = this.maxTags - this.tags.length;
    //}
}

