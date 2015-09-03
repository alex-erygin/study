var BlogViewModel = function() {
    var self = this;
    self.selectedPost = ko.observable();
    self.posts = ko.observableArray();

	var Post = function() {
		var date = ko.observable();
		var title = ko.observable();
		var description = ko.observable();
		var content = ko.observable();
		var comments = ko.observableArray()
	};

	
	function Init() {
		for (var i = 1; i <= 10; i++) {
			var post = new Post();
			post.date = new Date().toJSON().slice(0,10);
			post.title = "Пост №" + i;
			post.description = "Этот пост о " + i;
			post.content = i;
			
			self.posts.push(post);
		};
	};

	Init();

	function SelectPost(blogPost){
		self.selectedPost(blogPost);
		console.log(self.selectedPost());
	}


	return {
		posts : self.posts,
		selectPost : SelectPost,
		selectedPost : self.selectedPost
	};
}