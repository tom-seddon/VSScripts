# make_icons can only generate 100 icons, since it only does
# digits. (things will all work OK with >100 scripts, but script100
# will have the same icon as script00.)
def max_num_scripts():
    return 100
    
def base_id():
    return 512
    
