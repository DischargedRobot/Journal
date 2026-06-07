import Divider from "@mui/material/Divider"
import ListItem from "@mui/material/ListItem"
import ListItemIcon from "@mui/material/ListItemIcon"
import ListItemText from "@mui/material/ListItemText"
import { Fragment } from "react/jsx-runtime"
import { useState } from "react"
import ExpandMoreIcon from "@mui/icons-material/ExpandMore"

export interface Item {
	text: string
	key?: string
	icon?: React.ReactNode
	onClick?: () => void
	items?: Omit<Item, "items" | "icon">[]
}

interface Props {
	item: Item
	isSelected: boolean
	onSelect: (item: Item) => void
}

const SideBarItem = ({ item, isSelected, onSelect }: Props) => {
	const hasItems = item.items && item.items.length > 0

	const [isOpen, setIsOpen] = useState(false)

	const [selectedItem, setSelectedItem] = useState<Item | null>(null)
	const [selectedSubItem, setSelectedSubItem] = useState<Item | null>(null)

	return (
		<Fragment>
			<ListItem
				className="cursor-pointer"
				onClick={() => {
					if (hasItems) {
						setIsOpen(isSelected ? !isOpen : true)
					}
					onSelect(item)
					setSelectedSubItem(null)
					item.onClick?.()
				}}
				sx={
					isSelected
						? {
								borderLeftWidth: 5,
								borderColor: "primary.main",
							}
						: {
								opacity: 0.5,
							}
				}
			>
				{item.icon && <ListItemIcon>{item.icon}</ListItemIcon>}
				<ListItemText
					slotProps={{
						primary: {
							className: "title ",
						},
					}}
				>
					{item.text}
				</ListItemText>
				{hasItems && (
					<ListItemIcon>
						<ExpandMoreIcon
							className="transition-transform duration-300"
							fontSize="large"
							sx={
								isOpen && isSelected
									? { transform: "rotate(180deg)" }
									: {}
							}
							onClick={() => setIsOpen(!isOpen)}
						/>
					</ListItemIcon>
				)}
			</ListItem>
			{hasItems &&
				isOpen &&
				isSelected &&
				item.items!.map((item) => (
					<Fragment key={item.key ?? item.text}>
						<ListItem
							sx={
								selectedSubItem?.key === item.key
									? {
											backgroundColor: "secondary.light",
											color: "contrastingSecondary.main",
										}
									: {
											backgroundColor: "secondary.main",
											color: "secondary.contrastText",
										}
							}
							onClick={() => setSelectedSubItem(item)}
						>
							{selectedSubItem === item && (
								<ListItemIcon>
									<ExpandMoreIcon
										fontSize="large"
										sx={
											selectedSubItem === item
												? {
														transform:
															"rotate(270deg)",
														color: "primary.main",
													}
												: {
														color: "contrastingSecondary.light",
													}
										}
									/>
								</ListItemIcon>
							)}
							<ListItemText>{item.text}</ListItemText>
						</ListItem>
					</Fragment>
				))}
			<Divider
				sx={{
					borderColor: "contrastingSecondary.light",
				}}
			/>
		</Fragment>
	)
}

export default SideBarItem
