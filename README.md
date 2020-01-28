# regen
Regen - Next Generation DNA Sequencing Tool
====

Regen is the a next generation DNA Sequencing tool in the same vein of ElPrep or SamTools.


Why and When to use Regen?
====

ReGen is a new DNA sequencing tool based on being state of the art high volume DNA processing.

Why not using SamTools or ElPrep? 
====

This is a biased comparison, but in our scientific paper we show that we could do processing faster than ElPrep algorithm (version 3) 
and use much less memory. We achieve this by extracting parallelism and reducing memory usage.

Read here if you are curious:
https://github.com/ciplogic/elprep-study/blob/master/paper/elprep-study.pdf

Compared with previous paper, ReGen has more improvements: 

* it uses even less memory

* it uses .Net Core, and this in turn it runs even faster than the original Java implementation (before comparing Java with .Net, 
.Net uses the section "Future work" from the referenced paper of before, so if that work would be done in Java, there 
is the possibility that Java would run faster or similar)

For people that are not aware what ElPrep is and how it separates from SamTools, the main idea is that: 
ElPrep keeps in memory the whole SAM/BAM file and it does run all algorithms in memory where the speeds up comes from.

So, if you have huge data sets, but also a lot of memory, and if ElPrep doesn't fit you because you don't have 256 GB of memory, 
but you have a "just 64GB" machine, Regen is for you: based on our original prototype (and when the whole program would be finished)
the performance would be likely faster than ElPrep by a factor of 2 and memory usage would be less by a factor or 4 or 5.



