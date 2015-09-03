var BlogViewModel = function() {
    var self = this;
    self.selectedPost = ko.observable();
    self.posts = ko.observableArray();

	var Post = function() {
		var date = ko.observable();
		var title = ko.observable();
		var description = ko.observable();
		var content = ko.observable();
		var comments = ko.observableArray();
	};

	var Comment = function() {
		var content = ko.observable();
		var date = ko.observable();
	};	
	
	function getDate() {
		return new Date().toJSON().slice(0,10);
	};

	function Init() {
		for (var i = 1; i <= 10; i++) {
			var post = new Post();
			post.date = getDate();
			post.title = "Пост №" + i;
			post.description = "Этот пост о " + i;
			post.content = i;
			post.comments = ko.observableArray();
			self.posts.push(post);
		}

		self.posts().forEach(function(item){
			var comment = new Comment();
			comment.content = "Наказать жестачайшэ" + item.content;
			comment.date = getDate();
			item.comments.push(comment);			

			var comment = new Comment();
			comment.content = "Аффтор жжот! " + item.content;
			comment.date = getDate();
			item.comments.push(comment);			
		});
	};

	function SelectPost(blogPost) {
		self.selectedPost(blogPost);
		console.log(self.selectedPost());
	}

	function AddComment(options) {
		options.date = getDate();
		options.content = "omg";
		var comment = new Comment();
		comment.date = options.date;
		comment.content = options.content;
		self.selectedPost().comments.push(comment);
	}

	Init();

	return {
		posts : self.posts,
		selectPost : SelectPost,
		selectedPost : self.selectedPost,
		AddComment : AddComment
	};
};