# sort_words

punctuation = ",.?!#$%&()-+=/'\":;"
numbers = "1234567890"

def word_weight(word: str) -> int:
	score = 0
	for char in word:
		score += 1
		if char in numbers:
			score += 1
		if char in punctuation:
			score += 2

	return score


def sortwords(file: open, target: open):
	sorted_words = list()
	try:
		infile = open(file, "r")
		outfile = open(target, "w")

		for line in infile:
			sorted_words.append(line)
		
		for word in sorted(sorted_words, key = word_weight):
			outfile.write(word)
	finally:
		if infile:
			infile.close()


def main():
	source = input("Source file: ")
	target = input("Target file: ")
	sortwords(source, target)
	print("Done.")


if __name__ == "__main__":
	main()
